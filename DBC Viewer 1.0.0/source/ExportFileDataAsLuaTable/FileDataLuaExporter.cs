using PluginInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Text;

namespace ExportFileDataAsLuaTable
{
    [Export(typeof(IPlugin))]
    public class FileDataLuaExporter : IPlugin
    {
        [Import("PluginFinished")]
        public Action<int> Finished { get; set; }
        [Import("ClearDataTable")]
        public Action ClearDataTable { get; set; }

        public void Run(DataTable table)
        {
            if (table.TableName == "SoundKit.db2")
            {
                using (var sw1 = new StreamWriter("SoundKitData.lua"))
                {
                    sw1.WriteLine("local SOUNDDATA = {");

                    foreach (DataRow row in table.Rows)
                    {
                        sw1.WriteLine("[{0}] = \"{1}\"", row[16], row[0]);
                    }

                    sw1.WriteLine("}");
                }

                return;
            }

            if (table.TableName != "ManifestInterfaceData.db2")
                return;

            using (var sw = new StreamWriter("ManifestInterfaceData.txt"))
            {
                foreach (DataRow row in table.Rows)
                {
                    sw.WriteLine("{0}{1}", row[1], row[2]);
                }
            }
        }

        public void Run(IClientDBReader table)
        {
            if (Path.GetFileName(table.FileName) != "FileDataComplete.db2")
                return;

            var sw = new StreamWriter("FileDataLib.lua");

            sw.WriteLine("local PATHDATA = {");

            Dictionary<int, int> pathmap = new Dictionary<int, int>();

            int i1 = 0;

            Func<string, string, bool> filter = (path, name) =>
            {
                if (path.ToLower().StartsWith("world\\maps\\"))
                    return true;

                // .wwe, .pd4, .anim, .skin, .wmo
                string namel = name.ToLower();
                if (namel.EndsWith(".wwe") || namel.EndsWith(".pd4") || namel.EndsWith(".anim") || namel.EndsWith(".skin") || namel.EndsWith(".wmo") || namel.EndsWith(".html") || namel.EndsWith(".bone") || namel.EndsWith(".phys"))
                    return true;

                return false;
            };

            foreach (var row in table.Rows)
            {
                using (BinaryReader br = row)
                {
                    int id = br.ReadInt32();
                    int pathOffset = br.ReadInt32();
                    int nameOffset = br.ReadInt32();

                    string path = table.StringTable[pathOffset];
                    string name = table.StringTable[nameOffset];

                    if (filter(path, name))
                        continue;

                    if (!pathmap.ContainsKey(pathOffset))
                    {
                        pathmap[pathOffset] = i1;
                        sw.WriteLine("    [{0}] = \"{1}\",", i1, path.Replace("\\", @"\\"));
                        i1++;
                    }
                }
            }

            sw.WriteLine("}");

            sw.WriteLine("local NAMEDATA = {");

            Dictionary<int, int> namemap = new Dictionary<int, int>();

            int i2 = 0;

            List<string> batches = new List<string>();
            StringBuilder sb = new StringBuilder();
            int batchCount = 0;

            foreach (var row in table.Rows)
            {
                using (BinaryReader br = row)
                {
                    int id = br.ReadInt32();
                    int pathOffset = br.ReadInt32();
                    int nameOffset = br.ReadInt32();

                    string path = table.StringTable[pathOffset];
                    string name = table.StringTable[nameOffset];

                    if (filter(path, name))
                        continue;

                    if (!namemap.ContainsKey(nameOffset))
                    {
                        namemap[nameOffset] = i2;
                        //sw.WriteLine("    [{0}] = \"{1}\",", i2, name.Replace("\\", @"\\"));

                        batchCount++;

                        sb.AppendFormat("{0}:{1}", i2, name.Replace("\\", @"\\"));

                        if (batchCount != 5)
                            sb.Append(";");

                        if (batchCount == 5)
                        {
                            batches.Add(sb.ToString());
                            sb.Clear();
                            batchCount = 0;
                        }

                        i2++;
                    }
                }
            }

            if (batchCount != 0)
            {
                sb.Remove(sb.Length - 1, 1);
                batches.Add(sb.ToString());
            }

            foreach (var batch in batches)
            {
                sw.WriteLine("    \"{0}\",", batch);
            }

            batches.Clear();
            sb.Clear();
            batchCount = 0;

            sw.WriteLine("}");
            sw.WriteLine();

            sw.WriteLine("local NAMEDATA_UNPACKED = {}");
            sw.WriteLine();
            sw.WriteLine("do");
            sw.WriteLine("    for _, v in pairs(NAMEDATA) do");
            sw.WriteLine("        for _, n in pairs({strsplit(\";\", v)}) do");
            sw.WriteLine("            local id, name = strsplit(\":\", n)");
            sw.WriteLine("            NAMEDATA_UNPACKED[tonumber(id)] = name");
            sw.WriteLine("        end");
            sw.WriteLine("    end");
            sw.WriteLine("end");
            sw.WriteLine("table.wipe(NAMEDATA)");
            sw.WriteLine("NAMEDATA = nil");
            sw.WriteLine();

            sw.WriteLine("local FILEDATA = {");

            foreach (var row in table.Rows)
            {
                using (BinaryReader br = row)
                {
                    int id = br.ReadInt32();
                    int pathOffset = br.ReadInt32();
                    int nameOffset = br.ReadInt32();

                    string path = table.StringTable[pathOffset];
                    string name = table.StringTable[nameOffset];

                    if (filter(path, name))
                        continue;

                    //sw.WriteLine("    [{0}] = \"{1},{2}\",", id, pathmap[pathOffset], namemap[nameOffset]);

                    batchCount++;

                    sb.AppendFormat("{0}:{1},{2}", id, pathmap[pathOffset], namemap[nameOffset]);

                    if (batchCount != 5)
                        sb.Append(";");

                    if (batchCount == 5)
                    {
                        batches.Add(sb.ToString());
                        sb.Clear();
                        batchCount = 0;
                    }
                }
            }

            if (batchCount != 0)
            {
                sb.Remove(sb.Length - 1, 1);
                batches.Add(sb.ToString());
            }

            foreach (var batch in batches)
            {
                sw.WriteLine("    \"{0}\",", batch);
            }

            sw.WriteLine("}");
            sw.WriteLine();

            sw.WriteLine("local FILEDATA_UNPACKED = {}");
            sw.WriteLine();
            sw.WriteLine("do");
            sw.WriteLine("    for _, v in pairs(FILEDATA) do");
            sw.WriteLine("        for _, n in pairs({strsplit(\";\", v)}) do");
            sw.WriteLine("            local id, fd = strsplit(\":\", n)");
            sw.WriteLine("            FILEDATA_UNPACKED[tonumber(id)] = fd");
            sw.WriteLine("        end");
            sw.WriteLine("    end");
            sw.WriteLine("end");
            sw.WriteLine("table.wipe(FILEDATA)");
            sw.WriteLine("FILEDATA = nil");
            sw.WriteLine();

            sw.WriteLine("do");
            sw.WriteLine("    for file, v in pairs(FILEDATA_UNPACKED) do");
            sw.WriteLine("        local p, n = strsplit(\",\", v)");
            sw.WriteLine("        FILEDATA_UNPACKED[file] = PATHDATA[tonumber(p)] .. NAMEDATA_UNPACKED[tonumber(n)]");
            sw.WriteLine("    end");
            sw.WriteLine("end");
            sw.WriteLine("table.wipe(PATHDATA)");
            sw.WriteLine("PATHDATA = nil");
            sw.WriteLine("table.wipe(NAMEDATA_UNPACKED)");
            sw.WriteLine("NAMEDATA_UNPACKED = nil");
            sw.WriteLine();

            sw.WriteLine("function GetFileName(fileDataId)");
            sw.WriteLine("    return FILEDATA_UNPACKED[fileDataId]");
            sw.WriteLine("end");

            sw.Close();

            Finished(table.RecordsCount);
        }
    }
}
