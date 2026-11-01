using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;

namespace BCC.Modules.BCCForms.Components
{
    /// <summary>
    ///   Renders one page of pre-rendered list data as cards or through an HTML template
    ///   (header / item / footer with [Field] tokens). Values come from the same columns the grid
    ///   and XSL use, so links, dates, user names and number formats look the same everywhere.
    /// </summary>
    public class ListLayoutRenderer
    {
        static readonly Regex TokenPattern = new Regex(@"\[(?<name>[^\[\]\r\n]+?)\]", RegexOptions.Compiled);

        readonly DataTable _data;
        readonly List<FieldInfo> _fields = new List<FieldInfo>();
        readonly Dictionary<string, FieldInfo> _byTitle =
            new Dictionary<string, FieldInfo>(StringComparer.OrdinalIgnoreCase);
        readonly Func<object, string> _editLinkHtml;

        /// <param name="page">Pre-rendered DataSet holding the rows to show.</param>
        /// <param name="editLinkHtml">Edit icon link for a row's EditLink value ("" when not editable).</param>
        /// <param name="numberFormat">
        ///   Grid format string for a field ({0:...}), as the field type would set it on a grid column;
        ///   null for none. Lets cards and templates format numbers like the grid.
        /// </param>
        public ListLayoutRenderer(DataSet page, Func<object, string> editLinkHtml, Func<DataRow, string> numberFormat)
        {
            _data = page.Tables[DataSetTableName.Data];
            _editLinkHtml = editLinkHtml ?? (o => string.Empty);
            foreach (DataRow field in page.Tables[DataSetTableName.Fields].Rows)
            {
                var title = Convert.ToString(field[FieldsTableColumn.Title], CultureInfo.InvariantCulture);
                var info = new FieldInfo
                {
                    Title = title,
                    Visible = Convert.ToBoolean(field[FieldsTableColumn.Visible], CultureInfo.InvariantCulture),
                    IsSeparator = Convert.ToString(field[FieldsTableColumn.Type], CultureInfo.InvariantCulture) == "Separator",
                    ValueColumn = ValueColumnOf(field, title),
                    Format = numberFormat == null ? null : numberFormat(field)
                };
                _fields.Add(info);
                if (!_byTitle.ContainsKey(title)) _byTitle.Add(title, info);
            }
        }

        string ValueColumnOf(DataRow field, string title)
        {
            if (field.Table.Columns.Contains(FieldsTableColumn.ValueColumn) && !(field[FieldsTableColumn.ValueColumn] is DBNull))
            {
                var column = XmlConvert.DecodeName(Convert.ToString(field[FieldsTableColumn.ValueColumn], CultureInfo.InvariantCulture));
                if (_data.Columns.Contains(column)) return column;
            }
            return title;
        }

        /// <summary>One card per row: the visible fields as label / value pairs, plus the edit link.</summary>
        public string RenderCards()
        {
            var html = new StringBuilder("<div class=\"bccf-cards\">");
            foreach (DataRow row in _data.Rows)
            {
                html.Append("<article class=\"bccf-card\">");
                var edit = EditLink(row);
                if (edit.Length > 0) html.Append("<div class=\"bccf-card-edit\">").Append(edit).Append("</div>");
                html.Append("<dl class=\"bccf-card-fields\">");
                foreach (var field in _fields)
                {
                    if (!field.Visible || field.IsSeparator) continue;
                    var value = Display(row, field);
                    if (string.IsNullOrWhiteSpace(StripComments(value))) continue;
                    html.Append("<div class=\"bccf-card-field\"><dt>")
                        .Append(HttpUtility.HtmlEncode(field.Title))
                        .Append("</dt><dd>").Append(value).Append("</dd></div>");
                }
                html.Append("</dl></article>");
            }
            return html.Append("</div>").ToString();
        }

        /// <summary>
        ///   Header + one item per row + footer. Item tokens: [Field] (shown value, HTML), [Field:Text]
        ///   (plain text, encoded), [UDT:EditLink], [UDT:EditUrl], [UDT:RowId], [UDT:Index]. Header and
        ///   footer: [UDT:TotalRecords], [UDT:PageRecords]. Unknown tokens are left as they are (DNN
        ///   tokens such as [User:DisplayName] are replaced afterwards by the caller).
        /// </summary>
        public string RenderTemplate(string header, string item, string footer, int totalRecords)
        {
            var html = new StringBuilder();
            html.Append(ReplaceListTokens(header, totalRecords));
            var index = 0;
            foreach (DataRow row in _data.Rows)
            {
                index++;
                var rowIndex = index;
                html.Append(TokenPattern.Replace(item ?? string.Empty, m => RowToken(m, row, rowIndex)));
            }
            html.Append(ReplaceListTokens(footer, totalRecords));
            return html.ToString();
        }

        string ReplaceListTokens(string text, int totalRecords)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return TokenPattern.Replace(text, m =>
            {
                switch (m.Groups["name"].Value.ToLowerInvariant())
                {
                    case "udt:totalrecords":
                        return totalRecords.ToString(CultureInfo.CurrentCulture);
                    case "udt:pagerecords":
                        return _data.Rows.Count.ToString(CultureInfo.CurrentCulture);
                    default:
                        return m.Value;
                }
            });
        }

        string RowToken(Match m, DataRow row, int index)
        {
            var name = m.Groups["name"].Value;
            switch (name.ToLowerInvariant())
            {
                case "udt:editlink":
                    return EditLink(row);
                case "udt:editurl":
                    return HttpUtility.HtmlAttributeEncode(EditUrl(row));
                case "udt:rowid":
                    return Convert.ToString(row[DataTableColumn.RowId], CultureInfo.InvariantCulture);
                case "udt:index":
                    return index.ToString(CultureInfo.CurrentCulture);
            }

            FieldInfo field;
            if (_byTitle.TryGetValue(name, out field)) return Display(row, field);

            var colon = name.LastIndexOf(':');
            if (colon > 0 && string.Equals(name.Substring(colon + 1), "Text", StringComparison.OrdinalIgnoreCase) &&
                _byTitle.TryGetValue(name.Substring(0, colon), out field))
            {
                return HttpUtility.HtmlEncode(PlainText(row, field));
            }
            return m.Value;
        }

        string EditUrl(DataRow row)
        {
            return _data.Columns.Contains(DataTableColumn.EditLink)
                       ? Convert.ToString(row[DataTableColumn.EditLink], CultureInfo.InvariantCulture)
                       : string.Empty;
        }

        string EditLink(DataRow row)
        {
            var url = EditUrl(row);
            return string.IsNullOrEmpty(url) ? string.Empty : _editLinkHtml(url);
        }

        /// <summary>The value as the grid shows it (already safe HTML: links, sanitized text).</summary>
        string Display(DataRow row, FieldInfo field)
        {
            if (!_data.Columns.Contains(field.ValueColumn)) return string.Empty;
            var value = row[field.ValueColumn];
            if (value is DBNull || value == null) return string.Empty;
            if (!string.IsNullOrEmpty(field.Format) && value is IFormattable)
            {
                try
                {
                    return HttpUtility.HtmlEncode(string.Format(CultureInfo.CurrentCulture, field.Format, value));
                }
                catch (FormatException)
                {
                    // fall through to the plain value
                }
            }
            return value is string
                       ? (string) value
                       : HttpUtility.HtmlEncode(Convert.ToString(value, CultureInfo.CurrentCulture));
        }

        /// <summary>The stored value as plain text (no markup), for [Field:Text].</summary>
        string PlainText(DataRow row, FieldInfo field)
        {
            var original = field.Title + DataTableColumn.Appendix_Original;
            var column = _data.Columns.Contains(original) ? original : field.Title;
            if (!_data.Columns.Contains(column)) return string.Empty;
            var value = row[column];
            return value is DBNull || value == null
                       ? string.Empty
                       : Convert.ToString(value, CultureInfo.CurrentCulture);
        }

        static string StripComments(string html)
        {
            return Regex.Replace(html ?? string.Empty, "<!--.*?-->", string.Empty, RegexOptions.Singleline);
        }

        /// <summary>The format string a field type sets on a grid column, or null.</summary>
        public static string GridFormatFor(DataRow field)
        {
            try
            {
                var column = new BoundField();
                DataType.ByName(Convert.ToString(field[FieldsTableColumn.Type], CultureInfo.InvariantCulture))
                        .SetStylesAndFormats(column, Convert.ToString(field[FieldsTableColumn.OutputSettings], CultureInfo.InvariantCulture));
                return string.IsNullOrEmpty(column.DataFormatString) ? null : column.DataFormatString;
            }
            catch (Exception)
            {
                return null;
            }
        }

        sealed class FieldInfo
        {
            public string Title;
            public bool Visible;
            public bool IsSeparator;
            public string ValueColumn;
            public string Format;
        }
    }
}
