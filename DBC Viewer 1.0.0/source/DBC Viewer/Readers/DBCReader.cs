using PluginInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace DBCViewer
{
    class DBCReader : IClientDBReader
    {
        private const uint HeaderSize = 20;
        public const uint DBCFmtSig = 0x43424457;          // WDBC

        public int RecordsCount { get; private set; }
        public int FieldsCount { get; private set; }
        public int RecordSize { get; private set; }
        public int StringTableSize { get; private set; }

        public Dictionary<int, string> StringTable { get; private set; }

        private byte[] Strings;

        private byte[][] m_rows;

        public IEnumerable<BinaryReader> Rows
        {
            get
            {
                for (int i = 0; i < RecordsCount; ++i)
                {
                    yield return new BinaryReader(new MemoryStream(m_rows[i]), Encoding.UTF8);
                }
            }
        }

        public bool IsSparseTable { get { return false; } }
        public string FileName { get; private set; }

        public DBCReader(string fileName)
        {
            FileName = fileName;

            using (var reader = Extensions.FromFile(fileName))
            {
                if (reader.BaseStream.Length < HeaderSize)
                {
                    throw new InvalidDataException(string.Format("File {0} is corrupted!", fileName));
                }

                if (reader.ReadUInt32() != DBCFmtSig)
                {
                    throw new InvalidDataException(string.Format("File {0} isn't valid DBC file!", fileName));
                }

                RecordsCount = reader.ReadInt32();
                FieldsCount = reader.ReadInt32();
                RecordSize = reader.ReadInt32();
                StringTableSize = reader.ReadInt32();

                m_rows = new byte[RecordsCount][];

                for (int i = 0; i < RecordsCount; i++)
                    m_rows[i] = reader.ReadBytes(RecordSize);

                int stringTableStart = (int)reader.BaseStream.Position;

                StringTable = new Dictionary<int, string>();

                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    int index = (int)reader.BaseStream.Position - stringTableStart;
                    StringTable[index] = reader.ReadStringNull();
                }

                reader.BaseStream.Position = stringTableStart;
                Strings = reader.ReadBytes(StringTableSize);
            }
        }

        public string GetString(int offset)
        {
            unsafe
            {
                fixed (byte* b = Strings)
                {
                    int len = 0;

                    while (*(b + offset) != 0)
                        len++;

                    return new string((sbyte*)b, offset, len, Encoding.UTF8);
                }
            }
        }

        public void Save(DataTable table, Table def, string path)
        {
            using (var fs = new FileStream(path, FileMode.Create))
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                bw.Write(DBCFmtSig); // magic
                bw.Write(table.Rows.Count);
                bw.Write(FieldsCount);
                bw.Write(RecordSize);
                bw.Write(1); // stringTableSize placeholder

                var columnTypeCodes = table.Columns.Cast<DataColumn>().Select(c => Type.GetTypeCode(c.DataType)).ToArray();

                var stringLookup = new Dictionary<string, int>();
                stringLookup[""] = 0;

                var stringTable = new MemoryStream();
                stringTable.WriteByte(0);

                var fields = def.Fields;
                var fieldsCount = fields.Count;
                var fieldsArraySizes = fields.Select(f => f.ArraySize).ToArray();

                Func<TypeCode, bool> isSmallType = (t) =>
                {
                    if (t == TypeCode.SByte || t == TypeCode.Byte || t == TypeCode.Int16 || t == TypeCode.UInt16)
                        return true;
                    return false;
                };

                foreach (DataRow row in table.Rows)
                {
                    int colIndex = 0;

                    for (int j = 0; j < fieldsCount; j++)
                    {
                        int arraySize = fieldsArraySizes[j];

                        for (int k = 0; k < arraySize; k++)
                        {
                            switch (columnTypeCodes[colIndex])
                            {
                                case TypeCode.Byte:
                                    bw.Write(row.Field<byte>(colIndex));
                                    break;
                                case TypeCode.SByte:
                                    bw.Write(row.Field<sbyte>(colIndex));
                                    break;
                                case TypeCode.Int16:
                                    bw.Write(row.Field<short>(colIndex));
                                    break;
                                case TypeCode.UInt16:
                                    bw.Write(row.Field<ushort>(colIndex));
                                    break;
                                case TypeCode.Int32:
                                    bw.Write(row.Field<int>(colIndex));
                                    break;
                                case TypeCode.UInt32:
                                    bw.Write(row.Field<uint>(colIndex));
                                    break;
                                case TypeCode.Int64:
                                    bw.Write(row.Field<long>(colIndex));
                                    break;
                                case TypeCode.UInt64:
                                    bw.Write(row.Field<ulong>(colIndex));
                                    break;
                                case TypeCode.Single:
                                    bw.Write(row.Field<float>(colIndex));
                                    break;
                                case TypeCode.Double:
                                    bw.Write(row.Field<double>(colIndex));
                                    break;
                                case TypeCode.String:
                                    string str = row.Field<string>(colIndex);
                                    int offset;
                                    if (stringLookup.TryGetValue(str, out offset))
                                    {
                                        bw.Write(offset);
                                    }
                                    else
                                    {
                                        byte[] strBytes = Encoding.UTF8.GetBytes(str);
                                        if (strBytes.Length == 0)
                                        {
                                            throw new Exception("should not happen");
                                        }

                                        stringLookup[str] = (int)stringTable.Position;
                                        bw.Write((int)stringTable.Position);
                                        stringTable.Write(strBytes, 0, strBytes.Length);
                                        stringTable.WriteByte(0);
                                    }
                                    break;
                                default:
                                    throw new Exception("Unknown TypeCode " + columnTypeCodes[colIndex]);
                            }

                            // small fields are padded with zeros in old format versions if next field isn't small
                            long frem = ms.Position % 4;
                            if (frem != 0 && isSmallType(columnTypeCodes[colIndex]) && (colIndex + 1 < columnTypeCodes.Length) && !isSmallType(columnTypeCodes[colIndex + 1]))
                                ms.Position += (4 - frem);

                            colIndex++;
                        }
                    }

                    // padding at the end of the row
                    long rem = ms.Position % 4;
                    if (rem != 0)
                        ms.Position += (4 - rem);
                }

                // update stringTableSize in the header
                long oldPos = ms.Position;
                ms.Position = 0x10;
                bw.Write((int)stringTable.Length);
                ms.Position = oldPos;

                // write strings
                stringTable.Position = 0;
                stringTable.CopyTo(ms);

                // copy data to file
                ms.Position = 0;
                ms.CopyTo(fs);
            }
        }
    }
}
