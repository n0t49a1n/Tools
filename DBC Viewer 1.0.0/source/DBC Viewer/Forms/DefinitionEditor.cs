using PluginInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DBCViewer
{
    public partial class DefinitionEditor : Form
    {
        private DataGridViewRow rowToDrag;
        private bool m_changed;
        private bool m_saved;
        private MainForm m_mainForm;
        private Table editingTable;
        private Table origTable;

        public DefinitionEditor(MainForm mainForm)
        {
            m_mainForm = mainForm;

            InitializeComponent();
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            if (!m_changed)
                return;

            if (!CheckColumns())
            {
                MessageBox.Show("Column names aren't unique. Please fix them first.");
                return;
            }

            WriteXml();
            Close();
        }

        private bool CheckColumns()
        {
            var names = editingTable.Fields.Select(f => f.Name);
            if (names.Distinct().Count() != names.Count())
                return false;
            return true;
        }

        private void WriteXml()
        {
            Table newnode = new Table();
            newnode.Name = m_mainForm.DBCName;
            newnode.Build = Convert.ToInt32(buildTextBox.Text);
            newnode.Fields = new List<Field>();

            foreach (Field f in editingTable.Fields)
                newnode.Fields.Add(f.Clone());

            if (origTable == null || editingTable.Build != newnode.Build)
                m_mainForm.Definitions.Tables.Add(newnode);
            else
            {
                int index = m_mainForm.Definitions.Tables.IndexOf(origTable);
                m_mainForm.Definitions.Tables[index] = newnode;
            }

            DBFilesClient.Save(m_mainForm.Definitions);
            m_saved = true;
        }

        public void InitDefinitions()
        {
            origTable = m_mainForm.Definition;

            var def = origTable?.Clone();

            if (def == null)
            {
                DialogResult result = MessageBox.Show(this, string.Format("Table {0} missing definition. Create default definition?", m_mainForm.DBCName),
                    "Definition Missing!",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1);

                if (result != DialogResult.Yes)
                    return;

                def = CreateDefaultDefinition();
                if (def == null)
                {
                    MessageBox.Show(string.Format("Can't create default definitions for {0}", m_mainForm.DBCName));

                    def = new Table();
                    def.Name = m_mainForm.DBCName;
                    def.Fields = new List<Field>();
                }
            }

            InitForm(def);
        }

        private void InitForm(Table def)
        {
            editingTable = def;

            buildTextBox.Text = def.Build.ToString();

            for (int i = 0; i < def.Fields.Count; i++)
                def.Fields[i].Index = i;

            BindingSource source = new BindingSource();
            source.DataSource = def;
            editorDataGridView.DataSource = source;
            editorDataGridView.DataMember = "Fields";
        }

        private Table CreateDefaultDefinition()
        {
            var ext = Path.GetExtension(m_mainForm.DBCFile).ToUpperInvariant();

            if (ext != ".DBC" && ext != ".DB2") // only for dbc and db2, as other formats have no fields count stored
                return null;

            using (var br = new BinaryReader(new FileStream(m_mainForm.DBCFile, FileMode.Open)))
            {
                var magic = br.ReadUInt32();
                var recordsCount = br.ReadUInt32();
                var fieldsCount = br.ReadUInt32();
                var recordsize = br.ReadUInt32();

                if (magic == DBCReader.DBCFmtSig)
                {
                    // only for files with 4 byte fields (most of dbc's)
                    if ((recordsize % fieldsCount == 0) && (fieldsCount * 4 == recordsize))
                    {
                        var def = new Table();

                        def.Name = m_mainForm.DBCName;
                        def.Fields = new List<Field>();

                        for (int i = 0; i < fieldsCount; i++)
                        {
                            var field = new Field();

                            if (i == 0)
                            {
                                field.IsIndex = true;
                                field.Name = "m_ID";
                            }
                            else
                            {
                                field.Name = string.Format("field{0}", i);
                            }

                            field.Type = "int";

                            def.Fields.Add(field);
                        }

                        m_changed = true;
                        return def;
                    }
                }
                else if (magic == DB5Reader.DB5FmtSig)
                {
                    br.BaseStream.Position = 0;

                    DB5Reader db5 = new DB5Reader(br, true);

                    var def = new Table();

                    def.Name = m_mainForm.DBCName;
                    def.Fields = new List<Field>();

                    for (int i = 0; i < db5.Meta.Count; i++)
                    {
                        var field = new Field();

                        if (i == db5.IdIndex)
                        {
                            field.IsIndex = true;
                            field.Name = "m_ID";
                            field.Type = "int";
                        }
                        else
                        {
                            field.Name = string.Format("field{0:X2}", db5.Meta[i].Offset);

                            int bits = db5.Meta[i].Bits;

                            if (bits == 0x18)
                                field.Type = "byte";
                            else if (bits == 0x10)
                                field.Type = "ushort";
                            else if (bits == 0x08)
                                field.Type = "int";
                            else if (bits == 0x00)
                                field.Type = "int";
                            else if (bits == -32)
                                field.Type = "ulong";
                            else
                                throw new Exception("New Bits value detected!");

                            int byteCount = (32 - bits) >> 3;

                            // array
                            if (i + 1 < db5.Meta.Count && db5.Meta[i].Offset + byteCount != db5.Meta[i + 1].Offset)
                            {
                                int arraySize = (db5.Meta[i + 1].Offset - db5.Meta[i].Offset) / byteCount;

                                field.ArraySize = arraySize;
                            }

                            // array (last field)
                            int rowSize = db5.HasIndexTable ? db5.RecordSize + 4 : db5.RecordSize;
                            if (i + 1 == db5.Meta.Count && db5.Meta[i].Offset + byteCount != rowSize)
                            {
                                int diff = rowSize - db5.Meta[i].Offset;

                                if (diff >= byteCount * 2)
                                {
                                    int arraySize = diff / byteCount;

                                    field.ArraySize = arraySize;
                                }
                            }
                        }

                        def.Fields.Add(field);
                    }

                    m_changed = true;
                    return def;
                }
                else if (magic == DB6Reader.DB6FmtSig)
                {
                    br.BaseStream.Position = 0;

                    DB6Reader db6 = new DB6Reader(br, true);

                    var def = new Table();

                    def.Name = m_mainForm.DBCName;
                    def.Fields = new List<Field>();

                    for (int i = 0; i < db6.Meta.Count; i++)
                    {
                        var field = new Field();

                        if (i == db6.IdIndex)
                        {
                            field.IsIndex = true;
                            field.Name = "m_ID";
                            field.Type = "int";
                        }
                        else
                        {
                            field.Name = string.Format("field{0:X2}", db6.Meta[i].Offset);

                            int bits = db6.Meta[i].Bits;

                            if (bits == 0x18)
                                field.Type = "byte";
                            else if (bits == 0x10)
                                field.Type = "ushort";
                            else if (bits == 0x08)
                                field.Type = "int";
                            else if (bits == 0x00)
                                field.Type = "int";
                            else if (bits == -32)
                                field.Type = "ulong";
                            else
                                throw new Exception("New Bits value detected!");

                            int byteCount = (32 - bits) >> 3;

                            // array
                            if (i + 1 < db6.Meta.Count && db6.Meta[i].Offset + byteCount != db6.Meta[i + 1].Offset)
                            {
                                int arraySize = (db6.Meta[i + 1].Offset - db6.Meta[i].Offset) / byteCount;

                                field.ArraySize = arraySize;
                            }

                            // array (last field)
                            int rowSize = db6.HasIndexTable ? db6.RecordSize + 4 : db6.RecordSize;
                            if (i + 1 == db6.Meta.Count && db6.Meta[i].Offset + byteCount != rowSize)
                            {
                                int diff = rowSize - db6.Meta[i].Offset;

                                if (diff >= byteCount * 2)
                                {
                                    int arraySize = diff / byteCount;

                                    field.ArraySize = arraySize;
                                }
                            }
                        }

                        def.Fields.Add(field);
                    }

                    m_changed = true;
                    return def;
                }
            }

            return null;
        }

        private void DefinitionEditorNew_Load(object sender, EventArgs e)
        {
            editorDataGridView.AutoGenerateColumns = false;

            InitDefinitions();
        }

        private void editorDataGridView_DragDrop(object sender, DragEventArgs e)
        {
            Point clientPoint = editorDataGridView.PointToClient(new Point(e.X, e.Y));
            int dragToIndex = editorDataGridView.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            if (dragToIndex == -1 || rowToDrag.Index == -1 || dragToIndex == rowToDrag.Index)
                return;

            Field removed = editingTable.Fields[rowToDrag.Index];
            editingTable.Fields.RemoveAt(rowToDrag.Index);
            editingTable.Fields.Insert(dragToIndex, removed);

            InitForm(editingTable);
        }

        private void editorDataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            int rowToDragIndex = editorDataGridView.HitTest(e.X, e.Y).RowIndex;

            if (rowToDragIndex == -1)
                return;

            rowToDrag = editorDataGridView.Rows[rowToDragIndex];
            editorDataGridView.DoDragDrop(rowToDrag, DragDropEffects.Move);
        }

        private void editorDataGridView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void DefinitionEditorNew_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (m_changed)
                {
                    if (m_saved)
                        DialogResult = DialogResult.OK;
                    else
                        DialogResult = DialogResult.Cancel;
                }
                else
                    DialogResult = DialogResult.Abort;
            }
        }

        private void editorDataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            m_changed = true;
        }

        private void editorDataGridView_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            m_changed = true;
        }

        private void editorDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            m_changed = true;
        }

        private void editorDataGridView_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[0].Value = e.Row.Index;
            e.Row.Cells[1].Value = string.Format("field{0}", e.Row.Index);
            e.Row.Cells[2].Value = "int";
            e.Row.Cells[4].Value = 1;
        }
    }
}
