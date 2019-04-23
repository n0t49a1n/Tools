using PluginInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace DBCViewer
{
    class ADBReader : IClientDBReader
    {
        private const int HeaderSize = 48;
        private const uint ADBFmtSig = 0x32484357;          // WCH2
        private const uint ADB5FmtSig = 0x35484357;          // WCH5

        public int RecordsCount { get; private set; }
        public int FieldsCount { get; private set; }
        public int RecordSize { get; private set; }
        public int StringTableSize { get; private set; }

        public Dictionary<int, string> StringTable { get; private set; }

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

        public ADBReader(string fileName)
        {
            FileName = fileName;

            using (var reader = Extensions.FromFile(fileName))
            {
                if (reader.BaseStream.Length < HeaderSize)
                {
                    throw new InvalidDataException(string.Format("File {0} is corrupted!", fileName));
                }

                var signature = reader.ReadUInt32();

                if (signature != ADBFmtSig && signature != ADB5FmtSig)
                {
                    throw new InvalidDataException(string.Format("File {0} isn't valid DBC file!", fileName));
                }

                RecordsCount = reader.ReadInt32();
                FieldsCount = reader.ReadInt32(); // not fields count in WCH2
                RecordSize = reader.ReadInt32();
                StringTableSize = reader.ReadInt32();

                // WCH2 specific fields
                uint tableHash = reader.ReadUInt32(); // new field in WCH2
                uint build = reader.ReadUInt32(); // new field in WCH2

                int timestamp_last_written = reader.ReadInt32(); // Unix time in WCH2
                int min_id = reader.ReadInt32(); // new field in WCH2
                int max_id = reader.ReadInt32(); // new field in WCH2 (index table?)
                int locale = reader.ReadInt32(); // new field in WCH2

                if(signature == ADBFmtSig)
                {
                    int unk5 = reader.ReadInt32(); // new field in WCH2

                    if (max_id != 0)
                    {
                        reader.ReadBytes(max_id * 4 - HeaderSize);     // an index for rows
                        reader.ReadBytes(max_id * 2 - HeaderSize * 2); // a memory allocation bank
                    }
                }
                else
                {

                }

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
            }
        }

        public void Save(DataTable table, Table def, string path)
        {
            throw new NotImplementedException();
        }
    }
}
