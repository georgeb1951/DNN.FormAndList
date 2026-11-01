using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using BCC.Modules.BCCForms.Components;
using BCC.Modules.BCCForms.Interfaces;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using DotNetNuke.UI.WebControls;
using Globals = DotNetNuke.Common.Globals;
using System.Text;
using System.Globalization;
using System.Web;
using DotNetNuke.Abstractions.ClientResources;
using DotNetNuke.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using DotNetNuke.Abstractions.Portals;

namespace BCC.Modules.BCCForms
{
    public partial class EditForm : PortalModuleBase, IActionable, IFormEvents
    {
        private readonly INavigationManager navigationManager;
        private readonly IPortalAliasService portalAliasService;

        public EditForm()
        {
            // In DNN 10 we can use constructor injection for dependencies.
            this.navigationManager = this.DependencyProvider.GetRequiredService<INavigationManager>();
            this.portalAliasService = this.DependencyProvider.GetRequiredService<IPortalAliasService>();
        }

        EditControls _editControls;
        int _userDefinedRowId;
        CaptchaControl _ctlCaptcha;
        bool _hasUpdatePermission;
        bool _hasDeletePermission;

        readonly IDictionary<Label, Control> _labelcontrols = new Dictionary<Label, Control>();
        readonly IDictionary<PropertyLabelControl, Control> _propertylabelcontrols = new Dictionary<PropertyLabelControl, Control>();
        FormDataController _dataController;
        FormDataController DataController =>  _dataController ?? (_dataController = new FormDataController(ModuleContext, this.navigationManager, this.portalAliasService));

        public string JsUiDatePattern => Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern
                                     .ToLower()
                                     .Replace("yyyy", "yy");

        DataSet _data;
        DataSet Data
        {
            get
            {
                if (_data == null)
                {
                    if (!int.TryParse(Request.QueryString[DataTableColumn.RowId], out _userDefinedRowId))
                    {
                        _userDefinedRowId = Convert.ToInt32(-1);
                    }
                    _data = DataController.GetRow(_userDefinedRowId);
                }
                return _data;
            }
        }

        DataRow CurrentRow
        {
            get
            {
                if (Data.Tables[DataSetTableName.Data].Rows.Count == 0)
                {
                    var r = Data.Tables[DataSetTableName.Data].NewRow();
                    r[DataTableColumn.RowId] = _userDefinedRowId;
                    Data.Tables[DataSetTableName.Data].Rows.Add(r);
                }
                return Data.Tables[DataSetTableName.Data].Rows[0];
            }
        }

        Components.Settings _settings;
        new Components.Settings Settings => _settings ?? (_settings = new Components.Settings(ModuleContext.Settings)); 

        bool IsNewRow => _userDefinedRowId == -1; 

        #region Private Methods
        void BuildTemplateForm(IEnumerable<FormColumnInfo> editForm, string template)
        {
            var tr = new TokenReplaceForForms();
            var controlstring = tr.GetControlTemplate(template);
            var control = Page.ParseControl(controlstring);
            foreach (var currentField in editForm)
            {
                var controlId = currentField.Title.SafeId();
                var editcontrolPlaceholder = FindControlRecursive(control, $"editor_for_{ controlId}");
                if (editcontrolPlaceholder != null && currentField.EditControl != null) editcontrolPlaceholder.Controls.Add(currentField.EditControl);
                var labelPlaceholder = FindControlRecursive(control, $"label_for_{controlId}");
                if (labelPlaceholder != null) labelPlaceholder.Controls.Add(GetLabel(currentField.Title, currentField.Help, currentField.EditControl));

            }
            EditFormPlaceholder.Visible = true;
            // hook for module.css: labels and inputs placed outside DNN's dnnFormItem rows
            EditFormPlaceholder.Attributes["class"] = "bccf-template";
            EditFormPlaceholder.Controls.Add(control);

        }

        private Control FindControlRecursive(Control rootControl, string controlID)
        {
            if (rootControl.ID == controlID)
                return rootControl;

            foreach (Control controlToSearch in rootControl.Controls)
            {
                Control controlToReturn = FindControlRecursive(controlToSearch, controlID);
                if (controlToReturn != null)
                    return controlToReturn;
            }
            return null;
        }

        void BuildCssForm(IEnumerable<FormColumnInfo> editForm)
        {
            EditFormPlaceholder.Visible = true;
            Control currentContainer = EditFormPlaceholder;
            foreach (var currentField in editForm)
            {
                if (currentField.IsCollapsible)
                {
                    EditFormPlaceholder.Controls.Add(GetSeparatorFormPattern(currentField.Title, true));
                    var fieldset = new HtmlGenericControl("fieldset");
                    currentContainer = fieldset;
                    fieldset.Visible = currentField.Visible;
                    EditFormPlaceholder.Controls.Add(currentContainer);
                }
                else if (currentField.IsSeparator && currentField.Visible)
                {

                    EditFormPlaceholder.Controls.Add(GetSeparatorFormPattern(currentField.Title));
                    currentContainer = EditFormPlaceholder;
                }
                else
                {
                    var divFormItem = new HtmlGenericControl("div");
                    // DNN's form pattern marks required fields on the row (theme-styled asterisk after the label)
                    divFormItem.Attributes.Add("class", currentField.Required ? "dnnFormItem dnnFormRequired" : "dnnFormItem");
                    divFormItem.Controls.Add(GetLabel(currentField.Title, currentField.Help, currentField.EditControl));
                    if (currentField.EditControl != null)
                    {
                        divFormItem.Controls.Add(currentField.EditControl);
                    }
                    divFormItem.Visible = currentField.Visible;
                    if (!currentField.IsUserDefinedField) currentContainer = EditFormPlaceholder;
                    currentContainer.Controls.Add(divFormItem);
                }
            }
        }


        Control GetLabel(string title, string help, Control editcontrol)
        {

            if (help == string.Empty)
            {
                var l = new Label
                {
                    Text = string.Format("<span>{0}</span>", title),
                    AssociatedControlID = editcontrol.ID
                };

                var d = new HtmlGenericControl("div");
                // same markup and class as DNN's own label (div.dnnLabel > label > span), minus the help
                // icon, so DNN and the theme lay out every label of the form identically
                d.Attributes.Add("class", "dnnLabel bccf-label-nohelp");
                d.Controls.Add(l);

                _labelcontrols.Add(l, editcontrol);
                return d;
            }
            // built through DI so DNN 10's PropertyLabelControl gets its services (parameterless ctor is deprecated)
            var label = ActivatorUtilities.CreateInstance<PropertyLabelControl>(this.DependencyProvider);
            label.ID = string.Format("{0}_label", XmlConvert.EncodeName(title));
            label.Caption = title;
            label.HelpText = help;
            label.EditControl = editcontrol;
            _propertylabelcontrols.Add(label, editcontrol);
            return label;
        }

        static Control GetSeparatorFormPattern(string title, bool expendable = false)
        {
            return title == string.Empty
                ? new LiteralControl("<h2 class=\"dnnFormSectionHead\"/>")
                : (expendable
                    ? new LiteralControl(string.Format("<h2 class=\"dnnFormSectionHead\"><a>{0}</a></h2>", title))
                    : new LiteralControl(string.Format("<h2 class=\"dnnFormSectionHead\">{0}</h2>", title)));
        }
        void CheckPermission(bool isUsersOwnItem = true)
        {
            var security = new ModuleSecurity(ModuleContext);
            if (
                !((!IsNewRow && security.IsAllowedToEditRow(isUsersOwnItem)) ||
                  (IsNewRow && security.IsAllowedToAddRow() && (security.IsAllowedToAdministrateModule() || HasAddPermissonByQuota()))))
            {
                if (IsNested())
                {
                    cmdUpdate.Enabled = false;

                    divForm.Visible = true;
                }
                else
                {
                    Response.Redirect(this.navigationManager.NavigateURL(ModuleContext.TabId), true);
                }
            }
            else
            {
                _hasUpdatePermission = true;
            }
            _hasDeletePermission = Convert.ToBoolean(security.IsAllowedToDeleteRow(isUsersOwnItem) && !IsNewRow);
            cmdDelete.Visible = _hasDeletePermission;
        }

        bool IsNested()
        {
            return (Parent.Parent) is PortalModuleBase;
        }

        bool HasAddPermissonByQuota()
        {
            var userquota = Settings.UserRecordQuota;
            if (userquota > 0 && Request.IsAuthenticated)
            {
                var ds = DataController.GetDataSet(false);
                return ModuleSecurity.HasAddPermissonByQuota(ds.Tables[DataSetTableName.Fields],
                                                             ds.Tables[DataSetTableName.Data], userquota,
                                                             UserInfo.GetSafeUsername());
            }
            return true;
        }

        void CheckPermission(string createdBy)
        {
            CheckPermission(ModuleContext.PortalSettings.UserInfo.Username == createdBy &&
                            createdBy != Definition.NameOfAnonymousUser);
        }

        bool IsAnonymous
        {
            get { return ModuleContext.PortalSettings.UserId == -1; }
        }

        /// <summary>DNN's built-in image CAPTCHA is shown to anonymous users.</summary>
        bool CaptchaNeeded()
        {
            return IsAnonymous && Settings.CaptchaMode == CaptchaMode.DnnCaptcha;
        }

        /// <summary>A reCAPTCHA or Turnstile token is required from anonymous users.</summary>
        bool ProviderCaptchaNeeded()
        {
            return IsAnonymous && CaptchaVerifier.UsesProviderToken(Settings.CaptchaMode);
        }

        bool HoneypotNeeded()
        {
            return IsAnonymous && Settings.UseHoneypot;
        }

        string HoneypotFieldName
        {
            get { return "bccf_hp_" + ModuleId.ToString(CultureInfo.InvariantCulture); }
        }

        /// <summary>
        ///   A text box placed off-screen: people never see or reach it (no tab stop, hidden from
        ///   screen readers), but form-filling bots tend to put something in it.
        /// </summary>
        void RenderHoneypot()
        {
            var name = HoneypotFieldName;
            phHoneypot.Controls.Add(new LiteralControl(
                "<div class=\"bccf-hp\" aria-hidden=\"true\" " +
                "style=\"position:absolute!important;left:-10000px!important;top:auto!important;width:1px;height:1px;overflow:hidden;\">" +
                "<label for=\"" + name + "\">" +
                HttpUtility.HtmlEncode(Localization.GetString("HoneypotLabel", LocalResourceFile)) +
                "</label><input type=\"text\" id=\"" + name + "\" name=\"" + name +
                "\" value=\"\" tabindex=\"-1\" autocomplete=\"off\" /></div>"));
        }

        /// <summary>Shows the reCAPTCHA / Turnstile widget and wires the client script for this module.</summary>
        void RegisterProviderCaptcha()
        {
            gRecaptcha.Visible = true;
            ModuleServices.Get<IClientResourceController>()
                          .CreateScript("~/DesktopModules/BCCForms/js/bccforms.captcha.js")
                          .Register();

            Func<string, string> js = value => HttpUtility.JavaScriptStringEncode(value ?? string.Empty, true);
            Func<string, string> text = key => js(Localization.GetString(key, LocalResourceFile));
            var config = new StringBuilder();
            config.Append("(window.BCCFormsCaptchaQueue = window.BCCFormsCaptchaQueue || []).push({");
            config.Append("mode:").Append(js(Settings.CaptchaMode.ToString()));
            config.Append(",siteKey:").Append(js(Settings.ReCaptchaSiteKey));
            config.Append(",action:").Append(js(CaptchaVerifier.ExpectedAction));
            config.Append(",language:").Append(js(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName));
            config.Append(",container:").Append(js(gRecaptcha.ClientID));
            config.Append(",tokenField:").Append(js(hfCaptchaToken.ClientID));
            config.Append(",button:").Append(js(cmdUpdate.ClientID));
            config.Append(",text:{prompt:").Append(text("CaptchaPrompt.Text"));
            config.Append(",notReady:").Append(text("CaptchaNotReady.Text"));
            config.Append(",failed:").Append(text("CaptchaFailed.Text")).Append("}");
            config.Append("});");
            Page.ClientScript.RegisterStartupScript(typeof (EditForm), "bccf-captcha-" + ModuleId,
                                                    config.ToString(), true);
        }

        void ShowUponSubmit()
        {
            var message = new HtmlGenericControl("div")
            { InnerHtml = Settings.SubmissionText };
            message.Attributes["class"] = "dnnFormMessage dnnFormSuccess";
            MessagePlaceholder.Controls.Add(message);
        }

        void BuildEditForm()
        {
            var fieldSettingsTable = FieldSettingsController.GetFieldSettingsTable(ModuleId);
            var editForm = new List<FormColumnInfo>();
            FormColumnInfo currentField;
            var security = new ModuleSecurity(ModuleContext);

            _editControls = new EditControls(ModuleContext);

            foreach (DataRow dr in Data.Tables[DataSetTableName.Fields].Rows)
            {
                var fieldTitle = dr[FieldsTableColumn.Title].AsString();
                var dataTypeName = dr[FieldsTableColumn.Type].AsString();
                var dataType = DataType.ByName(dataTypeName);

                var isColumnEditable =
                    Convert.ToBoolean((!dataType.SupportsHideOnEdit ||
                                       Convert.ToBoolean(dr[FieldsTableColumn.ShowOnEdit])) &&
                                      (!Convert.ToBoolean(dr[FieldsTableColumn.IsPrivate]) ||
                                       security.IsAllowedToEditAllColumns()));

                //If Column is hidden, the Fieldtype falls back to "String" as the related EditControl works perfect even if it is not visibile
                //EditControls of other user defined datatypes may use core controls (e.g. UrlControl or RTE) which are not rock solid regarding viewstate.
                if (!isColumnEditable && dataType.IsUserDefinedField)
                {
                    dataTypeName = "String";
                }

                currentField = new FormColumnInfo { IsUserDefinedField = dataType.IsUserDefinedField };

                if (dataType.IsSeparator)
                {
                    var fieldId = (int)dr[FieldsTableColumn.Id];
                    currentField.IsCollapsible = Data.Tables[DataSetTableName.FieldSettings].GetFieldSetting("IsCollapsible", fieldId).AsBoolean();
                    currentField.IsSeparator = true;
                    if (dr[FieldsTableColumn.Visible].AsBoolean())
                    {
                        currentField.Title = fieldTitle;
                    }
                    currentField.Visible = isColumnEditable;
                }
                else
                {
                    currentField.Help = dr[FieldsTableColumn.HelpText].AsString();
                    currentField.Title = dr[FieldsTableColumn.Title].AsString();
                    currentField.Required =
                        Convert.ToBoolean(dr[FieldsTableColumn.Required].AsBoolean() &&
                                          dataType.IsUserDefinedField);

                    //advanced Settings: Dynamic control
                    currentField.EditControl = _editControls.Add(dr[FieldsTableColumn.Title].AsString(),
                                                                dataTypeName, Convert.ToInt32(dr[FieldsTableColumn.Id]),
                                                                dr[FieldsTableColumn.HelpText].AsString(),
                                                                dr[FieldsTableColumn.Default].AsString(),
                                                                dr[FieldsTableColumn.Required].AsBoolean(),
                                                                dr[FieldsTableColumn.ValidationRule].AsString(),
                                                                dr[FieldsTableColumn.ValidationMessage].AsString(),
                                                                dr[FieldsTableColumn.EditStyle].AsString(),
                                                                dr[FieldsTableColumn.InputSettings].AsString(),
                                                                dr[FieldsTableColumn.OutputSettings].AsString(),
                                                                dr[FieldsTableColumn.NormalizeFlag].AsBoolean(),
                                                                dr[FieldsTableColumn.MultipleValues].AsBoolean(),
                                                                fieldSettingsTable,
                                                                this);
                    currentField.Visible = isColumnEditable;
                }
                editForm.Add(currentField);
            }

            if (CaptchaNeeded())
            {
                _ctlCaptcha = new CaptchaControl
                {
                    ID = "Captcha",
                    CaptchaWidth = Unit.Pixel(130),
                    CaptchaHeight = Unit.Pixel(40),
                    ToolTip = Localization.GetString("CaptchaToolTip", LocalResourceFile),
                    ErrorMessage = Localization.GetString("CaptchaError", LocalResourceFile)
                };
                currentField = new FormColumnInfo
                {
                    Title = Localization.GetString("Captcha", LocalResourceFile),
                    EditControl = _ctlCaptcha,
                    Visible = true,
                    IsUserDefinedField = false
                };
                editForm.Add(currentField);
            }

            var enableFormTemplate = Settings.EnableFormTemplate;
            var formTemplate = Settings.FormTemplate;
            if (enableFormTemplate && !string.IsNullOrEmpty(formTemplate))
                BuildTemplateForm(editForm, formTemplate);
            else
                BuildCssForm(editForm);
            //Change captions of buttons in Form mode
            if (IsNewRow && Settings.ListOrForm.Contains("Form"))
            {
                cmdUpdate.Attributes["resourcekey"] = "cmdSend.Text";
            }
        }

        #endregion

        #region Event Handlers


        protected override void OnInit(EventArgs e)
        {
            // the form can be opened on its own edit page, where nothing else registers jQuery UI (date pickers)
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(
                this.DependencyProvider.GetRequiredService<DotNetNuke.Abstractions.Application.IApplicationStatusInfo>(),
                this.DependencyProvider.GetRequiredService<DotNetNuke.Abstractions.Logging.IEventLogger>(),
                this.PortalSettings,
                DotNetNuke.Framework.JavaScriptLibraries.CommonJs.jQueryUI);
            Page.RegisterRequiresViewStateEncryption();
            BuildEditForm();
            cmdCancel.Click += cmdCancel_Click;
            cmdDelete.Click += cmdDelete_Click;
            cmdUpdate.Click += cmdUpdate_Click;

            Load += Page_Load;
            PreRender += EditForm_PreRender;
        }

        void EditForm_PreRender(object sender, EventArgs e)
        {
            foreach (var labelcontrol in _labelcontrols)
            {
                var label = labelcontrol.Key;
                var control = (EditControl)labelcontrol.Value;
                if (control.ValueControl != null) label.AssociatedControlID = control.ValueControl.ID;
            }
            foreach (var labelcontrol in _propertylabelcontrols)
            {
                var label = labelcontrol.Key;
                var control = labelcontrol.Value as EditControl;
                if (control != null && control.ValueControl != null) label.EditControl = control.ValueControl;
            }
        }

        void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (ProviderCaptchaNeeded())
                {
                    RegisterProviderCaptcha();
                }
                if (HoneypotNeeded())
                {
                    RenderHoneypot();
                }
                if (Page.IsPostBack == false)
                {
                    EnsureActionButton();
                    ClientAPI.AddButtonConfirm(cmdDelete, Localization.GetString("DeleteItem", LocalResourceFile));                    
                }

                if (!IsNewRow)
                {
                    if (!Page.IsPostBack)
                    {
                        //Clear all default values
                        foreach (var edit in _editControls.Values)
                        {
                            edit.Value = "";
                        }
                    }
                    foreach (DataRow field in Data.Tables[DataSetTableName.Fields].Rows)
                    {
                        var dataTypeName = field[FieldsTableColumn.Type].AsString();
                        var dataType = DataType.ByName(dataTypeName);
                        var value = CurrentRow[field[FieldsTableColumn.Title].ToString()].ToString();
                        if (!dataType.IsSeparator)
                        {
                            if (!Page.IsPostBack)
                            {
                                _editControls[field[FieldsTableColumn.Title].ToString()].Value = value;
                            }
                            if (field[FieldsTableColumn.Type].ToString() == "CreatedBy")
                            {
                                CheckPermission(value);
                            }
                        }
                    }
                }
                else //New Entry
                {
                    //Default Values already have been set in BuildEditForms
                    cmdDelete.Visible = false;
                    CheckPermission();
                    if (!Page.IsPostBack && Request.QueryString["OnSubmit"].AsInt() == ModuleContext.ModuleId)
                    {
                        ShowUponSubmit();
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
                CheckPermission(false);
            }
        }

        void cmdCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (Settings.ListOrForm.Contains("Form") && IsNewRow)
                {
                    Response.Redirect(Request.RawUrl);
                }
                else
                {
                    Response.Redirect(this.navigationManager.NavigateURL(ModuleContext.TabId), true);
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        void cmdUpdate_Click(object sender, EventArgs e)
        {
            if (_hasUpdatePermission)
            {
                try
                {
                    //warning message of validation has failed
                    var warningMessage = string.Empty;
                    warningMessage = _editControls.Values.Where(edit => !edit.IsValid())
                        .Aggregate(warningMessage,
                            (current, edit) => current + string.Format(
                                                    "<li><b>{0}</b><br />{1}</li>",
                                                    edit.FieldTitle,
                                                    edit.ValidationMessage));
                    if (CaptchaNeeded() && !_ctlCaptcha.IsValid)
                    {
                        warningMessage += string.Format("<li><b>{0}</b><br />{1}</li>",
                                                        Localization.GetString("Captcha.Text", LocalResourceFile),
                                                        Localization.GetString("CaptchaError.Text", LocalResourceFile));
                    }

                    if (ProviderCaptchaNeeded())
                    {
                        var token = hfCaptchaToken.Value;
                        hfCaptchaToken.Value = string.Empty; // tokens are single-use
                        if (!CaptchaVerifier.Verify(Settings.CaptchaMode, token, Settings.ReCaptchaSecretKey,
                                                    Settings.ReCaptchaMinScore))
                        {
                            warningMessage += string.Format("<li><b>{0}</b><br />{1}</li>",
                                Localization.GetString("ReCaptcha.Text", LocalResourceFile),
                                Localization.GetString("ReCaptchaError.Text", LocalResourceFile));
                        }
                    }

                    if (HoneypotNeeded() && !string.IsNullOrEmpty(Request.Form[HoneypotFieldName]))
                    {
                        // Deliberately vague: don't tell a bot which check it failed.
                        warningMessage += string.Format("<li><b>{0}</b><br />{1}</li>",
                            Localization.GetString("ReCaptcha.Text", LocalResourceFile),
                            Localization.GetString("SpamRejected.Text", LocalResourceFile));
                    }

                    if (warningMessage == string.Empty)
                    {
                        //'Save values for every field separately
                        foreach (var edit in _editControls.Values)
                        {
                            var value = edit.Value;
                            CurrentRow[edit.FieldTitle] = value;
                        }

                        DataController.UpdateRow(Data);
                        RecordUpdated();

                        switch (Settings.ListOrForm)
                        {
                            case "List":
                                Response.Redirect(this.navigationManager.NavigateURL(ModuleContext.TabId), true);
                                break;
                            case "FormAndList":
                            case "ListAndForm":
                                var url = IsNewRow
                                              ? Request.RawUrl
                                              : this.navigationManager.NavigateURL(ModuleContext.TabId);
                                Response.Redirect(url,
                                                  true);
                                break;
                            case "Form":
                                switch (Settings.UponSubmitAction)
                                {
                                    case "Text":
                                        divForm.Visible = false;
                                        ShowUponSubmit();
                                        break;
                                    case "Form":
                                        Response.Redirect(
                                            this.navigationManager.NavigateURL(ModuleContext.TabId, "",
                                                                string.Format("OnSubmit={0}", ModuleId)), true);
                                        break;
                                    default:
                                        var strRedirectUrl = Settings.UponSubmitRedirect ?? this.navigationManager.NavigateURL(ModuleContext.TabId);
                                        Response.Redirect(Globals.LinkClick(strRedirectUrl, ModuleContext.TabId,
                                                                            ModuleContext.ModuleId));
                                        break;
                                }
                                break;
                        }
                    }
                    else
                    {
                        var moduleControl = (PortalModuleBase)(((Parent.Parent) is PortalModuleBase) ? Parent.Parent : this);
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(moduleControl, string.Format("<ul class=\"bccf-messages\">{0}</ul>", warningMessage),
                                                       ModuleMessage.ModuleMessageType.RedError);
                    }
                }
                catch (Exception exc) //Module failed to load
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                }
            }
        }

        void cmdDelete_Click(object sender, EventArgs e)
        {
            if (_hasDeletePermission)
            {
                try
                {
                    DataController.DeleteRow(_userDefinedRowId);
                    RecordDeleted();
                    Response.Redirect(this.navigationManager.NavigateURL(ModuleContext.TabId), true);
                }
                catch (Exception exc) //Module failed to load
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                }
            }
        }

        #endregion

        #region Optional Interfaces
        public void EnsureActionButton()
        {
            var useButtons = Settings.UseButtonsInForm;
            var sec = new ModuleSecurity(ModuleId, TabId, Settings);
            if (sec.IsAllowedToViewList() && Settings.OnlyFormIsShown)
            {
                var url = this.navigationManager.NavigateURL(TabId, "", "show=records");
                var title = Localization.GetString("List.Action", LocalResourceFile);
                cmdShowRecords.NavigateUrl = url;
                cmdShowRecords.Text = title;
                cmdShowRecords.Visible = useButtons;
            }

        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Add the "ManageUDT" link to the current action list.
        /// </summary>
        /// <returns>ModuleActionCollection</returns>
        /// -----------------------------------------------------------------------------
        public ModuleActionCollection ModuleActions
        {
            get
            {
                var useButtons = Settings.UseButtonsInForm;
                var cmdName = useButtons ? "" : ModuleActionType.AddContent;
                var actions = new ModuleActionCollection();
                var sec = new ModuleSecurity(ModuleId, TabId, Settings);
                if (sec.IsAllowedToViewList() && Settings.OnlyFormIsShown)
                {
                    var url = this.navigationManager.NavigateURL(TabId, "", "show=records");
                    var title = Localization.GetString("List.Action", LocalResourceFile);
                    actions.Add(ModuleContext.GetNextActionID(),
                                title, cmdName,
                                "", Utilities.IconURL("View"), url, false, SecurityAccessLevel.View, true, false);
                }
                return actions;
            }
        }
        public event Action RecordUpdated = delegate { };
        public event Action RecordDeleted = delegate { };
        #endregion

    }
}