using System.Collections.Generic;
using System.Data;
using System.IO;

namespace PluginInterface
{
    public interface IClientDBReader
    {
        string FileName { get; }
        int RecordsCount { get; }
        int FieldsCount { get; }
        int RecordSize { get; }
        int StringTableSize { get; }
        bool IsSparseTable { get; }
        Dictionary<int, string> StringTable { get; }
        IEnumerable<BinaryReader> Rows { get; }
        void Save(DataTable table, Table def, string path);
    }
}
