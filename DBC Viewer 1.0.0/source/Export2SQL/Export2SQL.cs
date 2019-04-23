using PluginInterface;
using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Export2SQL
{
    [Export(typeof(IPlugin))]
    public class Export2SQL : IPlugin
    {
        [Import("PluginFinished")]
        public Action<int> Finished { get; set; }
        [Import("ClearDataTable")]
        public Action ClearDataTable { get; set; }

        public void Run(DataTable data)
        {
            StreamWriter sqlWriter = new StreamWriter(Path.GetFileNameWithoutExtension(data.TableName) + ".sql");

            WriteSqlStructure(sqlWriter, data);

            foreach (DataRow row in data.Rows)
            {
                StringBuilder result = new StringBuilder();
                result.AppendFormat("INSERT INTO `dbc_{0}` VALUES (", Path.GetFileNameWithoutExtension(data.TableName));

                int flds = 0;

                for (var i = 0; i < data.Columns.Count; ++i)
                {
                    switch (data.Columns[i].DataType.Name)
                    {
                        case "Int64":
                            result.Append(row[i]);
                            break;
                        case "UInt64":
                            result.Append(row[i]);
                            break;
                        case "Int32":
                            result.Append(row[i]);
                            break;
                        case "UInt32":
                            result.Append(row[i]);
                            break;
                        case "Int16":
                            result.Append(row[i]);
                            break;
                        case "UInt16":
                            result.Append(row[i]);
                            break;
                        case "SByte":
                            result.Append(row[i]);
                            break;
                        case "Byte":
                            result.Append(row[i]);
                            break;
                        case "Single":
                            result.Append(((float)row[i]).ToString(CultureInfo.InvariantCulture));
                            break;
                        case "Double":
                            result.Append(((double)row[i]).ToString(CultureInfo.InvariantCulture));
                            break;
                        case "String":
                            result.Append("\"" + StripBadCharacters((string)row[i]) + "\"");
                            break;
                        default:
                            throw new Exception(string.Format("Unknown field type {0}!", data.Columns[i].DataType.Name));
                    }

                    if (flds != data.Columns.Count - 1)
                        result.Append(", ");

                    flds++;
                }

                result.Append(");");
                sqlWriter.WriteLine(result);
            }

            sqlWriter.Flush();
            sqlWriter.Close();

            Finished(data.Rows.Count);
        }

        private void WriteSqlStructure(StreamWriter sqlWriter, DataTable data)
        {
            sqlWriter.WriteLine("DROP TABLE IF EXISTS `dbc_{0}`;", Path.GetFileNameWithoutExtension(data.TableName));
            sqlWriter.WriteLine("CREATE TABLE `dbc_{0}` (", Path.GetFileNameWithoutExtension(data.TableName));

            for (var i = 0; i < data.Columns.Count; ++i)
            {
                sqlWriter.Write("\t" + string.Format("`{0}`", data.Columns[i].ColumnName));

                switch (data.Columns[i].DataType.Name)
                {
                    case "Int64":
                        sqlWriter.Write(" BIGINT NOT NULL DEFAULT '0'");
                        break;
                    case "UInt64":
                        sqlWriter.Write(" BIGINT UNSIGNED NOT NULL DEFAULT '0'");
                        break;
                    case "Int32":
                        sqlWriter.Write(" INT NOT NULL DEFAULT '0'");
                        break;
                    case "UInt32":
                        sqlWriter.Write(" INT UNSIGNED NOT NULL DEFAULT '0'");
                        break;
                    case "Int16":
                        sqlWriter.Write(" SMALLINT NOT NULL DEFAULT '0'");
                        break;
                    case "UInt16":
                        sqlWriter.Write(" SMALLINT UNSIGNED NOT NULL DEFAULT '0'");
                        break;
                    case "SByte":
                        sqlWriter.Write(" TINYINT NOT NULL DEFAULT '0'");
                        break;
                    case "Byte":
                        sqlWriter.Write(" TINYINT UNSIGNED NOT NULL DEFAULT '0'");
                        break;
                    case "Single":
                        sqlWriter.Write(" FLOAT NOT NULL DEFAULT '0'");
                        break;
                    case "Double":
                        sqlWriter.Write(" DOUBLE NOT NULL DEFAULT '0'");
                        break;
                    case "String":
                        sqlWriter.Write(" TEXT NOT NULL");
                        break;
                    default:
                        throw new Exception(string.Format("Unknown field type {0}!", data.Columns[i].DataType.Name));
                }

                sqlWriter.WriteLine(",");
            }

            foreach (DataColumn index in data.PrimaryKey)
            {
                sqlWriter.WriteLine("\tPRIMARY KEY (`{0}`)", index.ColumnName);
            }

            sqlWriter.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='Export of {0}';", data.TableName);
            sqlWriter.WriteLine();
        }

        static string StripBadCharacters(string input)
        {
            input = Regex.Replace(input, @"'", @"\'");
            input = Regex.Replace(input, @"\""", @"\""");
            return input;
        }

        public void Run(IClientDBReader data)
        {

        }
    }
}
