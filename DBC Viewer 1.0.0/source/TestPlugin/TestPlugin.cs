using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics;
using System.IO;
using PluginInterface;

namespace TestPlugin
{
    [Export(typeof(IPlugin))]
    public class FactionTemplateDebug : IPlugin
    {
        [Import("PluginFinished")]
        public Action<int> Finished { get; set; }
        [Import("ClearDataTable")]
        public Action ClearDataTable { get; set; }

        public void Run(DataTable data)
        {
            int count = 0;

            //StreamWriter sqlWriter = new StreamWriter(Path.GetFileNameWithoutExtension(data.TableName) + ".txt");

            //var lines = File.ReadAllLines("WorldMapOverlay.txt");

            //sqlWriter.WriteLine("local treasures = {");

            //var c = System.Globalization.CultureInfo.InvariantCulture;
            //System.Threading.Thread.CurrentThread.CurrentCulture = c;
            //// Sets the UI culture to French (France)
            //System.Threading.Thread.CurrentThread.CurrentUICulture = c;

            ClearDataTable();
            //var newrow = data.NewRow();
            //newrow[0] = "123";
            //newrow[1] = "456";
            //data.Rows.Add(newrow);

            using (var sr = new StreamReader("listfile_export.txt"))
            {
                string line;

                while((line = sr.ReadLine()) != null)
                {
                    var newrow = data.NewRow();
                    var tokens = line.Split(new [] { ' ' }, 2);
                    newrow[0] = tokens[0];
                    var file = tokens[1];
                    var slashIndex = file.LastIndexOf('\\');
                    if (slashIndex != -1)
                    {
                        var path1 = file.Substring(0, slashIndex + 1);
                        var path2 = file.Substring(slashIndex + 1);
                        newrow[1] = path1;
                        newrow[2] = path2;
                    }
                    else
                    {
                        newrow[1] = "";
                        newrow[2] = file;
                    }
                    data.Rows.Add(newrow);
                }
            }

            //foreach (DataRow row in data.Rows)
            //{
                //sqlWriter.WriteLine("{0} - {1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}{12:X2}{13:X2}{14:X2}{15:X2}{16:X2}", row[0],
                //    row[1], row[2], row[3], row[4], row[5], row[6], row[7], row[8],
                //    row[9], row[10], row[11], row[12], row[13], row[14], row[15], row[16]);

                //sqlWriter.WriteLine("{8} - {7:X2}{6:X2}{5:X2}{4:X2}{3:X2}{2:X2}{1:X2}{0:X2}", row[1], row[2], row[3], row[4], row[5], row[6], row[7], row[8], row[0]);

                //if ((int)row[4] == 1116 && (int)row[6] == 197 )
                //{
                //    sqlWriter.WriteLine("    [{0}] = {{ {1}, {2}, \"{3}\" }},", row[0], row[7].ToString(), row[8].ToString(), row[9]);
                //}

                //Interface\WorldMap\%s\%s%d.blp WorldMapArea[3], WorldMapArea[3], 1-12
                //Interface\WorldMap\%s\%s%d_%d.blp WorldMapArea[3], WorldMapArea[3], floor, 1-12

                //for (int j = 1; j <= 12; ++j)
                //{
                //    sqlWriter.WriteLine("Interface\\WorldMap\\{0}\\{0}{1}.blp", row[3], j);
                //}

                //for (int i = 0; i < 30; ++i)
                //{
                //    for (int j = 1; j <= 12; ++j)
                //    {
                //        sqlWriter.WriteLine("Interface\\WorldMap\\{0}\\{0}{1}_{2}.blp", row[3], i, j);
                //    }
                //}

                //for (int j = 1; j <= 12; ++j)
                //{
                //    sqlWriter.WriteLine("Interface\\WorldMap\\{0}\\{0}{1}.blp", row[10], j);
                //}

                //for (int i = 0; i < 30; ++i)
                //{
                //    for (int j = 1; j <= 12; ++j)
                //    {
                //        sqlWriter.WriteLine("Interface\\WorldMap\\{0}\\{0}{1}_{2}.blp", row[10], i, j);
                //    }
                //}

                //foreach(var line in lines)
                //{
                //    if (line.Contains("{" + row[0] + "}"))
                //        sqlWriter.WriteLine(line.Replace("{" + row[0] + "}", (string)row[3]));
                //}

                //for(int i = 0; i < 20; ++i)
                //    sqlWriter.WriteLine("Interface\\WorldMap\\{{{0}}}\\{1}{2}.blp", row[1], row[6], i);

                //string folder = (string)row[1];

                //sqlWriter.WriteLine("WORLD\\Maps\\{0}\\{0}_{1}_{2}.adt", folder, i, j);

                //for (int i = 0; i < 64; ++i)
                //{
                //    for (int j = 0; j < 64; ++j)
                //    {
                //        var a = "World\\minimaps\\{0}\\map{1}_{2}.blp";
                //        var b = "World\\minimaps\\{0}\\noliquid_map{1}_{2}.blp";

                //        sqlWriter.WriteLine(a, folder, i, j);
                //        sqlWriter.WriteLine(b, folder, i, j);
                //    }
                //}

                //sqlWriter.WriteLine("WORLD\\Maps\\{0}\\{0}.tex", folder);
                //sqlWriter.WriteLine("WORLD\\Maps\\{0}\\{0}.wdl", folder);
                //sqlWriter.WriteLine("WORLD\\Maps\\{0}\\{0}.wdt", folder);

                //for (int i = 0; i < 64; ++i)
                //{
                //    for (int j = 0; j < 64; ++j)
                //    {
                //        sqlWriter.WriteLine("WORLD\\Maps\\{0}\\{0}_{1}_{2}.adt", folder, i, j);
                //        sqlWriter.WriteLine("WORLD\\Maps\\{0}\\{0}_{1}_{2}_obj0.adt", folder, i, j);
                //        sqlWriter.WriteLine("WORLD\\Maps\\{0}\\{0}_{1}_{2}_obj1.adt", folder, i, j);
                //        sqlWriter.WriteLine("WORLD\\Maps\\{0}\\{0}_{1}_{2}_tex0.adt", folder, i, j);
                //        sqlWriter.WriteLine("WORLD\\Maps\\{0}\\{0}_{1}_{2}_tex1.adt", folder, i, j);
                //    }
                //}

                // FileData.dbc
                //sqlWriter.WriteLine((string)row[2] + (string)row[1]);

                // ManifestInterfaceData.dbc
                //sqlWriter.WriteLine((string)row[1] + (string)row[2]);

                //sqlWriter.WriteLine((string)row[1] + ".blp");
                //if ((string)row[4] != String.Empty)
                //    sqlWriter.WriteLine((string)row[4]);
                //if ((string)row[5] != String.Empty)
                //    sqlWriter.WriteLine((string)row[5]);
                //if ((string)row[6] != String.Empty)
                //    sqlWriter.WriteLine((string)row[6]);


                //uint flags = (uint)row[2];

                //uint reaction = ~(flags >> 12) & 2 | 1; // flags & 0x2000 ? 1 : 3

                //if (reaction != 0)
                //{
                //    Debug.Print("template {0}, reaction {1}", row[0], reaction);
                //    count++;
                //}
            //}

            //sqlWriter.WriteLine("}");

            //sqlWriter.Flush();
            //sqlWriter.Close();

            Finished(count);
        }

        public void Run(IClientDBReader data)
        {

        }
    }
}
