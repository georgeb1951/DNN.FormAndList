namespace BCC.Modules.BCCForms.Components
{
    /// <summary>How BCCForms_GetRowsPage orders a field's stored text (values match @SortType).</summary>
    public enum DatabaseSortType : byte
    {
        Text = 0,
        WholeNumber = 1,
        Decimal = 2,
        DateTime = 3,
        RowOrder = 4
    }
}
