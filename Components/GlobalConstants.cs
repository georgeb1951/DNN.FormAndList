using System;
using System.Collections.Generic;
using DotNetNuke.Services.Localization;

namespace BCC.Modules.BCCForms.Components
{
    public class RenderingMethod
    {
        public const string GridRendering = "DataGrid";
        public const string UserdefinedXSL = "XslUserDefinedStyleSheet";
        public const string PredefinedXSL = "XslPreDefinedStyleSheet";
    }


    public class Definition
    {
        public const string NameOfAnonymousUser = "Anonymous";
        public const string PathOfCustomConfig = "/Portals/_Default/BCCForms/";
        public const string PathOfModule = "/DesktopModules/BCCForms/";
        public const string ModuleDefinitionFriendlyName = "Form and List";
        public const string ModuleName = "BCCForms";
        public const string QueryStringParameter = "udt_{0}_param";
        public const string XSLFolderName = "XslStyleSheets";
        public const string TemplateFolderName = "templates";
        public const string horizontalRadioButtonEnabledToken = "-[[hRBL]]";
        public const string verticalRadioButtonEnabledToken = "-[[vRBL]]";

        public static readonly string SharedRessources = string.Format("~{0}{1}/SharedRescources.resx", PathOfModule,
                                                                       Localization.LocalResourceDirectory);
    }

    public class PermissionName
    {
        public const string HasEditRowPermission = "ROWEDIT";
        public const string HasDeleteRowPermission = "ROWDELETE";
        public const string HasAddRowPermission = "ROWADD";
        public const string ShowAllUserDefinedColumnsPermission = "COLUMNVISIBLE";
        public const string EditRestricedFieldsPermission = "PRIVATECOLUMNS";
        public const string ShowListPermission = "SHOWLIST";
        public const string Code = "BCCFORMS";
    }

    public class DataSetTableName
    {
        public const string Fields = "Fields";
        public const string FieldSettings = "FieldSettings";
        public const string Data = "Data";
        public const string Settings = "Settings";
        public const string TabSettings = "TabSettings";
        public const string Stylesheets = "XSL";
    }

    public class FieldsTableColumn
    {
        public const string Id = "UserDefinedFieldId";
        public const string Title = "FieldTitle";
        public const string Required = "Required";
        public const string Type = "FieldType";
        public const string HelpText = "HelpText";
        public const string Default = "Default";
        public const string Visible = "Visible";
        public const string ShowOnEdit = "ShowOnEdit";
        public const string ValueColumn = "ValueColumn";
        public const string SortColumn = "SortColumn";
        public const string Order = "FieldOrder";
        public const string Searchable = "Searchable";
        public const string IsPrivate = "PrivateField";
        public const string MultipleValues = "MultipleValues";
        public const string InputSettings = "InputSettings";
        public const string OutputSettings = "OutputSettings";
        public const string NormalizeFlag = "NormalizeFlag";
        public const string ValidationMessage = "ValidationMessage";
        public const string ValidationRule = "ValidationRule";
        public const string EditStyle = "EditStyle";
    }

    public class DataTypeNames
    {
        public const string UDT_DataType_CreatedBy = "CreatedBy";
        public const string UDT_DataType_CreatedAt = "CreatedAt";
        public const string UDT_DataType_ChangedBy = "ChangedBy";
        public const string UDT_DataType_ChangedAt = "ChangedAt";
        public const string UDT_DataType_String = "String";
    }

    public class DataTableColumn
    {
        public const string EditLink = "EditLink";
        public const string Value = "FieldValue";
        public const string RowId = "UserDefinedRowId";
        public const string Appendix_Prefix = "_UDT_";
        public const string Appendix_Url = Appendix_Prefix + "Url";
        public const string Appendix_Ticks = Appendix_Prefix + "Ticks";
        public const string Appendix_Original = Appendix_Prefix + "Original";
        public const string Appendix_LocalizedValue = Appendix_Prefix + "Value";
        public const string Appendix_Caption = Appendix_Prefix + "Caption";
    }

    public class SettingsTableColumn
    {
        public const string Value = "Value";
        public const string Setting = "Setting";
    }

    public class StylesheetTableColumn
    {
        public const string NameOfSetting = "Type";
        public const string LocalFilePath = "LocalFilePath";
        public const string Stylesheet = "Stylesheet";
    }

    /// <summary>
    ///   Module / tab-module setting keys. All use the BCCF_ prefix since 01.01.00; the Form and List
    ///   names they replaced are in <see cref="Legacy"/>, which the 01.01.00 upgrade script and the
    ///   template / import code use to carry old settings over.
    /// </summary>
    public static class SettingName
    {
        // List display
        public const string RenderingMethod = "BCCF_RenderingMethod";
        public const string Paging = "BCCF_Paging";
        public const string SortField = "BCCF_SortField";
        public const string SortOrder = "BCCF_SortOrder";
        public const string TopCount = "BCCF_TopCount";
        public const string Filter = "BCCF_Filter";
        public const string Search = "BCCF_Search";
        public const string ShowSearchTextBox = "BCCF_ShowSearchTextBox";
        public const string ShowNoRecordsUntilSearch = "BCCF_ShowNoRecordsUntilSearch";
        public const string SimpleSearch = "BCCF_SimpleSearch";
        public const string URLSearch = "BCCF_URLSearch";
        public const string XslPreDefinedStyleSheet = "BCCF_XslPreDefinedStyleSheet";
        public const string XslUserDefinedStyleSheet = "BCCF_XslUserDefinedStyleSheet";
        public const string ImageWidth = "BCCF_ImageWidth";
        public const string ImageHeight = "BCCF_ImageHeight";
        public const string URLNewWindow = "BCCF_URLNewWindow";
        public const string UserLinkNewWindow = "BCCF_UserLinkNewWindow";
        public const string UserLinkCaption = "BCCF_UserLinkCaption";
        public const string CnCLink = "BCCF_CnCLink";
        public const string ForceDownload = "BCCF_ForceDownload";
        public const string ShowSystemColumns = "BCCF_ShowSystemColumns";
        public const string ShowAllColumnsForAdmins = "BCCF_ShowAllColumnsForAdmins";
        public const string EditPrivateColumnsForAdmins = "BCCF_EditPrivateColumnsForAdmins";
        public const string ExcludeFromSearch = "BCCF_ExcludeFromSearch";

        // Form
        public const string ListOrForm = "BCCF_ListOrForm";
        public const string UseButtonsInForm = "BCCF_UseButtonsInForm";
        public const string FormTemplate = "BCCF_FormTemplate";
        public const string EnableFormTemplate = "BCCF_EnableFormTemplate";
        public const string CalculatedColumnsRenderExpressionInForm = "BCCF_CalculatedColumnsRenderExpressionInForm";
        public const string SubmissionText = "BCCF_SubmissionText";
        public const string UponSubmitAction = "BCCF_UponSubmitAction";
        public const string UponSubmitRedirect = "BCCF_UponSubmitRedirect";
        public const string EditOnlyOwnItems = "BCCF_EditOnlyOwnItems";
        public const string UserRecordQuota = "BCCF_UserRecordQuota";
        public const string ForceInputFiltering = "BCCF_ForceInputFiltering";

        // Spam protection. CaptchaMode supersedes ForceCaptchaForAnonymous + PreferReCaptcha (both still
        // read for modules saved before it existed). The two key settings hold the reCAPTCHA or
        // Turnstile keys, whichever provider is selected.
        public const string CaptchaMode = "BCCF_CaptchaMode";
        public const string ForceCaptchaForAnonymous = "BCCF_ForceCaptchaForAnonymous";
        public const string PreferReCaptcha = "BCCF_PreferReCaptcha";
        public const string ReCaptchaSiteKey = "BCCF_ReCaptchaSiteKey";
        public const string ReCaptchaSecretKey = "BCCF_ReCaptchaSecretKey";
        public const string ReCaptchaMinScore = "BCCF_ReCaptchaMinScore";
        public const string UseHoneypot = "BCCF_UseHoneypot";

        // Notification e-mail ("tracking")
        public const string TrackingEmailFrom = "BCCF_TrackingEmailFrom";
        public const string TrackingEmailReplyTo = "BCCF_TrackingEmailReplyTo";
        public const string TrackingEmailTo = "BCCF_TrackingEmailTo";
        public const string TrackingEmailCc = "BCCF_TrackingEmailCc";
        public const string TrackingEmailBcc = "BCCF_TrackingEmailBcc";
        public const string TrackingSubject = "BCCF_TrackingSubject";
        public const string TrackingScript = "BCCF_TrackingScript";
        public const string TrackingMessage = "BCCF_TrackingMessage";
        public const string TrackingTriggerOnNew = "BCCF_TrackingTriggerOnNew";
        public const string TrackingTriggerOnUpdate = "BCCF_TrackingTriggerOnUpdate";
        public const string TrackingTriggerOnDelete = "BCCF_TrackingTriggerOnDelete";
        public const string TrackingTextOnNew = "BCCF_TrackingTextOnNew";
        public const string TrackingTextOnUpdate = "BCCF_TrackingTextOnUpdate";
        public const string TrackingTextOnDelete = "BCCF_TrackingTextOnDelete";

        /// <summary>Pre-01.01.00 key (Form and List naming) → current key.</summary>
        public static readonly IReadOnlyDictionary<string, string> Legacy =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "UDT_RenderingMethod", RenderingMethod },
                { "UDT_Paging", Paging },
                { "UDT_SortField", SortField },
                { "UDT_SortOrder", SortOrder },
                { "UDT_TopCount", TopCount },
                { "UDT_Filter", Filter },
                { "UDT_Search", Search },
                { "UDT_ShowSearchTextBox", ShowSearchTextBox },
                { "UDT_ShowNoRecordsUntilSearch", ShowNoRecordsUntilSearch },
                { "UDT_SimpleSearch", SimpleSearch },
                { "UDT_URLSearch", URLSearch },
                { "UDT_XslPreDefinedStyleSheet", XslPreDefinedStyleSheet },
                { "UDT_XslUserDefinedStyleSheet", XslUserDefinedStyleSheet },
                { "UDT_ImageWidth", ImageWidth },
                { "UDT_ImageHeight", ImageHeight },
                { "UDT_URLNewWindow", URLNewWindow },
                { "UDT_UserLinkNewWindow", UserLinkNewWindow },
                { "UDT_UserLinkUserName", UserLinkCaption },
                { "UDT_CnCLink", CnCLink },
                { "UDT_ForceDownload", ForceDownload },
                { "UDT_ShowSystemColumns", ShowSystemColumns },
                { "UDT_ShowAllColumnsForAdmins", ShowAllColumnsForAdmins },
                { "UDT_EditPrivateColumnsForAdmins", EditPrivateColumnsForAdmins },
                { "UDT_ExcludeFromSearch", ExcludeFromSearch },
                { "UDT_ListNotFormsMode", ListOrForm },
                { "UseButtonsInForm", UseButtonsInForm },
                { "FormTemplate", FormTemplate },
                { "EnableFormTemplate", EnableFormTemplate },
                { "CalculatedColumnsRenderExpressionInForm", CalculatedColumnsRenderExpressionInForm },
                { "UDT_Submission_succeeded", SubmissionText },
                { "UDT_UponSubmitAction", UponSubmitAction },
                { "UDT_UponSubmitRedirect", UponSubmitRedirect },
                { "UDT_EditOnlyOwnItems", EditOnlyOwnItems },
                { "UDT_UserRecordQuota", UserRecordQuota },
                { "UDT_ForceFiltering", ForceInputFiltering },
                { "UDT_ForceCaptchaForAnonymous", ForceCaptchaForAnonymous },
                { "UDT_PreferReCaptcha", PreferReCaptcha },
                { "UDT_ReCaptchaSiteKey", ReCaptchaSiteKey },
                { "UDT_ReCaptchaPrivateKey", ReCaptchaSecretKey },
                { "UDT_TokenTrackingFrom", TrackingEmailFrom },
                { "UDT_TokenTrackingReplyTo", TrackingEmailReplyTo },
                { "UDT_TokenTracking", TrackingEmailTo },
                { "UDT_TokenTrackingCC", TrackingEmailCc },
                { "UDT_TokenTrackingBCC", TrackingEmailBcc },
                { "UDT_TrackingSubject", TrackingSubject },
                { "UDT_XslTracking", TrackingScript },
                { "UDT_TrackingMessage", TrackingMessage },
                { "UDT_TriggerOnNew", TrackingTriggerOnNew },
                { "UDT_TriggerOnUpdate", TrackingTriggerOnUpdate },
                { "UDT_TriggerOnDelete", TrackingTriggerOnDelete },
                { "UDT_TrackingTextOnNew", TrackingTextOnNew },
                { "UDT_TrackingTextOnUpdate", TrackingTextOnUpdate },
                { "UDT_TrackingTextOnDelete", TrackingTextOnDelete },
            };

        /// <summary>Returns the current key for <paramref name="key"/>, translating Form and List names.</summary>
        public static string Current(string key)
        {
            string current;
            return key != null && Legacy.TryGetValue(key, out current) ? current : key;
        }
    }
}