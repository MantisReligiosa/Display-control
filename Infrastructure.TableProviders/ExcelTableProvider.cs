using ExcelDataReader;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Infrastructure.TableProviders
{
    public class ExcelTableProvider : IExcelTableProvider
    {
        public IEnumerable<string> Header { get; private set; }

        public IEnumerable<IEnumerable<string>> Rows { get; private set; }

        private readonly Dictionary<TableType, Func<Stream, IExcelDataReader>> _readers = new Dictionary<TableType, Func<Stream, IExcelDataReader>>
        {
            {TableType.CSV, (s) => ExcelReaderFactory.CreateCsvReader(s,new ExcelReaderConfiguration
            {
                FallbackEncoding=System.Text.Encoding.GetEncoding(1251)
            }) },
            {TableType.Excel, (s) => ExcelReaderFactory.CreateReader(s) }
        };

        public void LoadData(object context, TableType tableType)
        {
            using (var stream = new MemoryStream(Convert.FromBase64String(context as string)))
            using (var reader = _readers[tableType].Invoke(stream))
            {
                var result = reader.AsDataSet();
                var workSheet = result.Tables[0];
                var headerRowIndex = 0;
                while (!workSheet.Rows[headerRowIndex].ItemArray.Any(i => i is string))
                    headerRowIndex++;
                var headerColumn = 0;
                while (!(workSheet.Rows[headerRowIndex].ItemArray[headerColumn] is string))
                    headerColumn++;
                var headerRow = workSheet.Rows[headerRowIndex].ItemArray.Skip(headerColumn);
                Header = headerRow.Select(o => o.ToString()).ToList();
                Rows = new List<List<string>>();
                for (int rowIndex = headerRowIndex + 1; rowIndex < workSheet.Rows.Count; rowIndex++)
                {
                    var rowStartColumn = 0;
                    while (!(workSheet.Rows[rowIndex].ItemArray[rowStartColumn] is string))
                        rowStartColumn++;
                    if (rowStartColumn < headerColumn)
                    {
                        var delta = headerColumn - rowStartColumn;
                        var topper = new List<string>();
                        for (int i = 0; i < delta; i++)
                        {
                            topper.Add(string.Empty);
                        }
                        var oldHeader = new List<string>(Header);
                        Header = new List<string>(topper.Union(oldHeader));
                        var newRows = new List<List<string>>();
                        foreach (var existRow in Rows)
                        {
                            var newRow = new List<string>(topper.Union(existRow));
                            newRows.Add(newRow);
                        }
                        Rows = new List<List<string>>(newRows);
                        headerColumn = rowStartColumn;
                    }
                    var row = workSheet.Rows[rowIndex].ItemArray.Skip(headerColumn);
                    ((List<List<string>>)Rows).Add(row.Select(o => o.ToString()).ToList());
                }
            }
        }
    }
}
