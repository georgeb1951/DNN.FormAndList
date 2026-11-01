using System;
using System.Collections.Generic;
using System.Data;

namespace BCC.Modules.BCCForms.Components
{
    public static class DataTableExtensions
    {
      static public DataTable Top(this DataTable dataTable, int topCount)
        {
            if (topCount > 0 && dataTable.Rows.Count > topCount)
            {
                KeepRange(dataTable, 0, topCount);
            }
            return dataTable;
        }

        static public DataTable FilterAndSort(this DataTable dataTable, string filter, string sortField, string sortOrder)
        {
            var sort = string.Empty;
            if (sortField != string.Empty && sortOrder != string.Empty)
            {
                sort = string.Format("{0} {1}", sortField, sortOrder);
            }
            var dv = new DataView(dataTable) {RowFilter = filter, Sort = sort};

            var filteredtable = dv.ToTable();
            dataTable.Clear();
            dataTable.Merge(filteredtable);
            return dataTable;
        }

        static public DataTable Page(this DataTable dataTable, int pageIndex, int pageSize)
        {
            if (pageSize > 0)
            {
                var first = Math.Max(0, pageIndex) * pageSize;
                if (first >= dataTable.Rows.Count)
                {
                    first = 0; // as before: an index past the end leaves the first rows
                }
                KeepRange(dataTable, first, pageSize);
            }
            return dataTable;
        }

        /// <summary>
        ///   Keeps rows [first, first + count) in one pass. (Removing rows one by one from the front
        ///   shifts the whole collection each time, which made late pages of big lists slow.)
        /// </summary>
        static void KeepRange(DataTable dataTable, int first, int count)
        {
            var rows = dataTable.Rows;
            var last = Math.Min(rows.Count, first + count);
            if (first == 0 && last == rows.Count) return;
            var keep = new List<object[]>(Math.Max(0, last - first));
            for (var index = first; index < last; index++)
            {
                var values = rows[index].ItemArray;
                foreach (DataColumn column in dataTable.Columns)
                {
                    if (column.Expression != string.Empty) values[column.Ordinal] = null; // computed again
                }
                keep.Add(values);
            }
            var readOnly = new List<DataColumn>();
            foreach (DataColumn column in dataTable.Columns)
            {
                if (column.ReadOnly && column.Expression == string.Empty) { readOnly.Add(column); column.ReadOnly = false; }
            }
            dataTable.BeginLoadData();
            rows.Clear();
            foreach (var values in keep)
            {
                dataTable.LoadDataRow(values, true);
            }
            dataTable.EndLoadData();
            foreach (var column in readOnly) column.ReadOnly = true;
        }
    }
}