using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Modules.HTMLEditorProvider;
using BCC.Modules.BCCForms.Components;

namespace BCC.Modules.BCCForms.DataTypes
{

    #region EditControl

    /// -----------------------------------------------------------------------------
    /// <summary>
    ///   Edit &amp; Validation Control for DataType "TextHtml"
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class EditTextHtml : EditControl
    {
        protected HtmlEditorProvider RichTextEditor;
        protected TextBox TxtBox;
        protected string DefValue = "";
        bool _useRichTextEditor;

        public override string Value
        {
            get
            {
                string returnValue = _useRichTextEditor ? HttpUtility.HtmlDecode(RichTextEditor.Text) : TxtBox.Text;
                if (FilterScript)
                {
                    returnValue = ApplyScriptFilter(returnValue);
                }
                if (FilterTags)
                {
                    returnValue = HtmlUtils.StripTags(returnValue, true);
                }
                //FCK returns <p>&#160;</p> if no input was made
                if (returnValue == "<p>&#160;</p>")
                {
                    returnValue = "";
                }
                return returnValue;
            }

            set
            {
                if (_useRichTextEditor)
                {
                    RichTextEditor.Text =value;
                }
                else
                {
                    TxtBox.Text = value;
                }
            }
        }

        public EditTextHtml()
        {
            Init += Page_Init;
        }

        void Page_Init(object sender, EventArgs e)
        {
            _useRichTextEditor = !GetFieldSetting("PlainText").AsBoolean() && !FilterTags;
            if (FilterScript)
            {
            }

            DefValue = DefaultValue;

            if (_useRichTextEditor)
            {

                var pnlEditor = new Panel {CssClass = "dnnLeft"};

                RichTextEditor = HtmlEditorProvider.Instance();
                var controlId = CleanID(string.Format("{0}", FieldTitle));
                RichTextEditor.ControlID = controlId;
                RichTextEditor.Initialize();
                //RichTextEditor.Height = ControlStyle.Height;
                //RichTextEditor.Width = ControlStyle.Width;
                if (RichTextEditor.Height.IsEmpty)
                {
                    RichTextEditor.Height = new Unit(250);
                }

                RichTextEditor.Width = new Unit(400);

                Controls.Clear();
                var htmlEditorControl = RichTextEditor.HtmlEditorControl;
                pnlEditor.Controls.Add(htmlEditorControl);
                RichTextEditor.Text = DefValue;
                Controls.Add(pnlEditor);
                ValueControl = FindControl(controlId );
            }
            else
            {
                TxtBox = new TextBox { TextMode = TextBoxMode.MultiLine, Rows = 7, Text = DefValue, ID = CleanID(FieldTitle) };
                Controls.Add(TxtBox);

                ValueControl = TxtBox;
            }
        }
    }

    #endregion

    #region DataType

    /// -----------------------------------------------------------------------------
    /// <summary>
    ///   MetaData and Formating for DataType "TextHtml"
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class DataTypeTextHtml : DataType
    {
        public override Components.DatabaseSortType? DatabaseSort
        {
            get { return Components.DatabaseSortType.Text; }
        }

        readonly FieldSettingType[] _fieldSettingTypes = new[]
                    {
                        new FieldSettingType {Key = "PlainText", Section = "Form", SystemType = "Boolean"}
                    };

        public override IEnumerable<FieldSettingType> FieldSettingTypes
        {
            get
            {
                return _fieldSettingTypes;
            }
        }

        /// <summary>Stored text may contain markup (editors may enter it on purpose, imports and older
        ///   data were never filtered), so it is sanitized before it is shown.</summary>
        public override void RenderValuesToHtmlInsideDataSet(DataSet ds, int moduleId, bool noScript)
        {
            SanitizeColumnsForDisplay(ds);
        }

        public override string Name
        {
            get { return "TextHtml"; }
        }

        public override bool SupportsDefaultValue
        {
            get { return true; }
        }

        public override bool SupportsEditing
        {
            get { return true; }
        }

        public override bool SupportsValidation
        {
            get { return true; }
        }

        public override bool SupportsSearch
        {
            get { return true; }
        }
    }

    #endregion
}