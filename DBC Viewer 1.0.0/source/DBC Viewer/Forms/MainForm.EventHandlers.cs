using PluginInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DBCViewer
{
    partial class MainForm
    {
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            LoadFile(openFileDialog1.FileName);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void dataGridView1_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1 || e.RowIndex >= m_dataTable.Rows.Count)
                return;

            ulong val = 0;

            Type dataType = m_dataTable.Columns[e.ColumnIndex].DataType;
            CultureInfo culture = CultureInfo.InvariantCulture;
            object value = dataGridView1[e.ColumnIndex, e.RowIndex].Value;

            if (dataType != typeof(string))
            {
                if (dataType == typeof(sbyte))
                    val = (ulong)Convert.ToSByte(value, culture);
                else if (dataType == typeof(byte))
                    val = Convert.ToByte(value, culture);
                else if (dataType == typeof(short))
                    val = (ulong)Convert.ToInt16(value, culture);
                else if (dataType == typeof(ushort))
                    val = Convert.ToUInt16(value, culture);
                else if (dataType == typeof(int))
                    val = (ulong)Convert.ToInt32(value, culture);
                else if (dataType == typeof(uint))
                    val = Convert.ToUInt32(value, culture);
                else if (dataType == typeof(long))
                    val = (ulong)Convert.ToInt64(value, culture);
                else if (dataType == typeof(ulong))
                    val = Convert.ToUInt64(value, culture);
                else if (dataType == typeof(float))
                    val = BitConverter.ToUInt32(BitConverter.GetBytes((float)value), 0);
                else if (dataType == typeof(double))
                    val = BitConverter.ToUInt64(BitConverter.GetBytes((double)value), 0);
                else
                    val = Convert.ToUInt32(value, culture);
            }
            else
            {
                if (m_dbreader.StringTable != null)
                    val = (uint)m_dbreader.StringTable.Where(kv => string.Compare(kv.Value, (string)value, StringComparison.Ordinal) == 0).Select(kv => kv.Key).FirstOrDefault();
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormatLine(culture, "Integer: {0:D}", val);
            sb.AppendFormatLine(new BinaryFormatter(), "HEX: {0:X}", val);
            sb.AppendFormatLine(new BinaryFormatter(), "BIN: {0:B}", val);
            sb.AppendFormatLine(culture, "Float: {0}", BitConverter.ToSingle(BitConverter.GetBytes(val), 0));
            sb.AppendFormatLine(culture, "Double: {0}", BitConverter.ToDouble(BitConverter.GetBytes(val), 0));

            string strValue;
            if (m_dbreader.StringTable != null && m_dbreader.StringTable.TryGetValue((int)val, out strValue))
            {
                sb.AppendFormatLine(culture, "String: {0}", strValue);
            }
            else
            {
                sb.AppendFormatLine(culture, "String: <empty>");
            }

            e.ToolTipText = sb.ToString();
        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell != null)
                label1.Text = string.Format(CultureInfo.InvariantCulture, "Current Cell: {0}x{1}", dataGridView1.CurrentCell.RowIndex, dataGridView1.CurrentCell.ColumnIndex);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            m_startTime = DateTime.Now;

            string file = (string)e.Argument;

            m_dbreader = DBReaderFactory.GetReader(file, m_definition);

            m_fields = new List<Field>(m_definition.Fields);

            string[] colNames = m_fields.Select(f => f.Name).ToArray();

            int[] arraySizes = m_fields.Select(f => f.ArraySize).ToArray();

            bool isDBCorDB2 = m_dbreader is DBCReader || m_dbreader is DB2Reader;

            m_dataTable = new DataTable(Path.GetFileName(file));
            m_dataTable.Locale = CultureInfo.InvariantCulture;

            CreateColumns();                                // Add columns

            CreateIndexes();                                // Add indexes

            TypeCode[] types = m_dataTable.Columns.Cast<DataColumn>().Select(c => Type.GetTypeCode(c.DataType)).ToArray();

            List<ColumnMeta> meta = null;

            if (m_dbreader is DB5Reader)
                meta = (m_dbreader as DB5Reader).Meta;

            if (m_dbreader is DB6Reader)
                meta = (m_dbreader as DB6Reader).Meta;

            Func<TypeCode, bool> isSmallType = (t) =>
            {
                if (t == TypeCode.SByte || t == TypeCode.Byte || t == TypeCode.Int16 || t == TypeCode.UInt16)
                    return true;
                return false;
            };

            foreach (var row in m_dbreader.Rows) // Add rows
            {
                DataRow dataRow = m_dataTable.NewRow();

                using (BinaryReader br = row)
                {
                    int colIndex = 0;

                    for (int j = 0; j < m_fields.Count; j++)    // Add cells
                    {
                        for (int k = 0; k < arraySizes[j]; k++)
                        {
                            switch (types[colIndex])
                            {
                                case TypeCode.SByte:
                                    dataRow.SetField(colIndex, br.ReadInt8(meta?[j]));
                                    break;
                                case TypeCode.Byte:
                                    dataRow.SetField(colIndex, br.ReadUInt8(meta?[j]));
                                    break;
                                case TypeCode.Int16:
                                    dataRow.SetField(colIndex, br.ReadInt16(meta?[j]));
                                    break;
                                case TypeCode.UInt16:
                                    dataRow.SetField(colIndex, br.ReadUInt16(meta?[j]));
                                    break;
                                case TypeCode.Int32:
                                    dataRow.SetField(colIndex, br.ReadInt32(meta?[j]));
                                    break;
                                case TypeCode.UInt32:
                                    dataRow.SetField(colIndex, br.ReadUInt32(meta?[j]));
                                    break;
                                case TypeCode.Int64:
                                    dataRow.SetField(colIndex, br.ReadInt64(meta?[j]));
                                    break;
                                case TypeCode.UInt64:
                                    dataRow.SetField(colIndex, br.ReadUInt64(meta?[j]));
                                    break;
                                case TypeCode.Single:
                                    dataRow.SetField(colIndex, br.ReadSingle(meta?[j]));
                                    break;
                                case TypeCode.Double:
                                    dataRow.SetField(colIndex, br.ReadDouble(meta?[j]));
                                    break;
                                case TypeCode.String:
                                    ReadStringField(colIndex, meta?[j], dataRow, br);
                                    break;
                                default:
                                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Unknown field type {0} for column {1}!", types[colIndex], colNames[j]));
                            }

                            // small fields are padded with zeros in old format versions if next field isn't small
                            long frem = br.BaseStream.Position % 4;
                            if (isDBCorDB2 && frem != 0 && isSmallType(types[colIndex]) && (colIndex + 1 < types.Length) && !isSmallType(types[colIndex + 1]))
                                br.BaseStream.Position += (4 - frem);

                            colIndex++;
                        }
                    }
                }

                m_dataTable.Rows.Add(dataRow);

                int percent = (int)((float)m_dataTable.Rows.Count / m_dbreader.RecordsCount * 100.0f);
                (sender as BackgroundWorker).ReportProgress(percent);
            }

            e.Result = file;
        }

        private void ReadStringField(int colIndex, ColumnMeta meta, DataRow dataRow, BinaryReader br)
        {
            if (m_dbreader is WDBReader)
                dataRow.SetField(colIndex, br.ReadStringNull());
            else if (m_dbreader is STLReader)
            {
                int offset = br.ReadInt32();
                dataRow.SetField(colIndex, (m_dbreader as STLReader).ReadString(offset));
            }
            else
            {
                try
                {
                    dataRow.SetField(colIndex, m_dbreader.IsSparseTable ? br.ReadStringNull() : m_dbreader.StringTable[br.ReadInt32(meta)]);
                }
                catch
                {
                    dataRow.SetField(colIndex, "Invalid string index!");
                }
            }
        }

        private void columnsFilterEventHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            dataGridView1.Columns[item.Name].Visible = !item.Checked;

            ((ToolStripMenuItem)item.OwnerItem).ShowDropDown();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Value = 0;

            if (e.Error != null)
            {
                if (e.Error is InvalidDataException)
                {
                    ShowErrorMessageBox(e.Error.ToString());
                    statusToolStripLabel.Text = "Error.";
                }
                else
                {
                    statusToolStripLabel.Text = "Error in definitions.";
                    StartEditor();
                }
            }
            else
            {
                TimeSpan total = DateTime.Now - m_startTime;
                statusToolStripLabel.Text = string.Format(CultureInfo.InvariantCulture, "Ready. Loaded in {0} sec", total.TotalSeconds);
                Text = string.Format(CultureInfo.InvariantCulture, "DBC Viewer - {0}", e.Result);
                SetDataSource(m_dataTable);
                InitColumnsFilter();
            }
        }

        private void filterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFilterForm();
        }

        private void ShowFilterForm()
        {
            if (m_dataTable == null)
                return;

            if (m_filterForm == null || m_filterForm.IsDisposed)
                m_filterForm = new FilterForm();

            if (!m_filterForm.Visible)
                m_filterForm.Show(this);
        }

        private void resetFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_dataTable == null)
                return;

            if (m_filterForm != null)
                m_filterForm.ResetFilters();

            SetDataSource(m_dataTable);
        }

        private void runPluginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_dataTable == null)
            {
                ShowErrorMessageBox("Nothing loaded yet!");
                return;
            }

            //m_catalog.Refresh();

            if (Plugins.Count == 0)
            {
                ShowErrorMessageBox("No plugins found!");
                return;
            }

            using (PluginsForm selector = new PluginsForm())
            {
                selector.SetPlugins(Plugins);
                DialogResult result = selector.ShowDialog(this);

                if (result != DialogResult.OK)
                {
                    ShowErrorMessageBox("No plugin selected!");
                    return;
                }

                if (selector.NewPlugin != null)
                    m_catalog.Catalogs.Add(new AssemblyCatalog(selector.NewPlugin));

                statusToolStripLabel.Text = "Plugin working...";
                Thread pluginThread = new Thread(() => RunPlugin(selector.PluginIndex));
                pluginThread.Start();
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            int columnIndex = e.ColumnIndex;
            int columnIndexFix = 0;

            for (int i = 0; i < m_fields.Count; i++)
            {
                for (int j = 0; j < m_fields[i].ArraySize; j++)
                {
                    if (columnIndex == columnIndexFix)
                    {
                        string format = m_fields[i].Format;

                        if (string.IsNullOrWhiteSpace(format))
                            return;

                        string fmtStr = "{0:" + format + "}";
                        e.Value = string.Format(new BinaryFormatter(), fmtStr, e.Value);
                        e.FormattingApplied = true;
                        return;
                    }

                    columnIndexFix++;
                }
            }
        }

        private void resetColumnsFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.Visible = true;
                ((ToolStripMenuItem)columnsFilterToolStripMenuItem.DropDownItems[col.Name]).Checked = false;
            }
        }

        private void autoSizeColumnsModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem control = (ToolStripMenuItem)sender;

            foreach (ToolStripMenuItem item in autoSizeModeToolStripMenuItem.DropDownItems)
                if (item != control)
                    item.Checked = false;

            int index = (int)columnContextMenuStrip.Tag;
            dataGridView1.Columns[index].AutoSizeMode = (DataGridViewAutoSizeColumnMode)Enum.Parse(typeof(DataGridViewAutoSizeColumnMode), (string)control.Tag);
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var index = (int)columnContextMenuStrip.Tag;
            dataGridView1.Columns[index].Visible = false;
            ((ToolStripMenuItem)columnsFilterToolStripMenuItem.DropDownItems[index]).Checked = true;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseFile();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            WindowState = Properties.Settings.Default.WindowState;
            Size = Properties.Settings.Default.WindowSize;
            Location = Properties.Settings.Default.WindowLocation;

            m_workingFolder = Application.StartupPath;
            dataGridView1.AutoGenerateColumns = true;

            Compose();

            string[] cmds = Environment.GetCommandLineArgs();
            if (cmds.Length > 1)
                LoadFile(cmds[1]);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.WindowState = WindowState;

            if (WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.WindowSize = Size;
                Properties.Settings.Default.WindowLocation = Location;
            }
            else
            {
                Properties.Settings.Default.WindowSize = RestoreBounds.Size;
                Properties.Settings.Default.WindowLocation = RestoreBounds.Location;
            }

            Properties.Settings.Default.Save();
        }

        private void difinitionEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_dbcName == null)
                return;

            StartEditor();
        }

        private void reloadDefinitionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDefinitions();
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            label2.Text = string.Format("Rows Displayed: {0}", dataGridView1.RowCount);

            if (m_dbreader != null)
                label3.Text = string.Format("Info: {0} fields, {1} records of size {2}, stringTable size {3}", m_dbreader.FieldsCount, m_dbreader.RecordsCount, m_dbreader.RecordSize, m_dbreader.StringTableSize);
            else
                label3.Text = "Info: ...";
        }

        private void dataGridView1_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                columnContextMenuStrip.Tag = e.ColumnIndex;

                foreach (ToolStripMenuItem item in autoSizeModeToolStripMenuItem.DropDownItems)
                {
                    if (item.Tag.ToString() == dataGridView1.Columns[e.ColumnIndex].AutoSizeMode.ToString())
                        item.Checked = true;
                    else
                        item.Checked = false;
                }

                e.ContextMenuStrip = columnContextMenuStrip;
            }
            else if (e.ColumnIndex != -1)
            {
                cellContextMenuStrip.Tag = Tuple.Create(e.ColumnIndex, e.RowIndex);
                e.ContextMenuStrip = cellContextMenuStrip;
            }
        }

        private void filterThisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tuple<int, int> meta = (Tuple<int, int>)cellContextMenuStrip.Tag;
            ShowFilterForm();
            m_filterForm.SetSelection(dataGridView1.Columns[meta.Item1].Name, dataGridView1[meta.Item1, meta.Item2].Value.ToString());
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutBox about = new AboutBox())
                about.ShowDialog();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = m_dataTable.TableName;

            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            dataGridView1.EndEdit();

            try
            {
                m_dbreader.Save(m_dataTable, m_definition, saveFileDialog1.FileName);
            }
            catch (Exception exc)
            {
                ShowErrorMessageBox(exc.Message);
            }
        }
    }
}
