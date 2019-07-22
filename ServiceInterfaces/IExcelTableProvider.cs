using System.Collections.Generic;

namespace ServiceInterfaces
{
    public interface IExcelTableProvider
    {
        IEnumerable<string> Header { get; }

        IEnumerable<IEnumerable<string>> Rows { get; }

        void LoadData(object context, TableType tableType);
    }

    public enum TableType
    {
        Unknown = 0,
        CSV = 1,
        Excel = 2
    }
}
