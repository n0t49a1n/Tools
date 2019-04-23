using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace DBCViewer
{
    public partial class FilterForm : Form
    {
        private DataTable m_filter;

        private object[] decimalOperators = new object[]
        {
            ComparisonType.And,
            ComparisonType.AndNot,
            ComparisonType.Equal,
            ComparisonType.NotEqual,
            ComparisonType.Less,
            ComparisonType.Greater
        };

        private object[] stringOperators = new object[]
        {
            ComparisonType.Equal,
            ComparisonType.NotEqual,
            ComparisonType.StartWith,
            ComparisonType.EndsWith,
            ComparisonType.Contains
        };

        private object[] floatOperators = new object[]
        {
            ComparisonType.Equal,
            ComparisonType.NotEqual,
            ComparisonType.Less,
            ComparisonType.Greater
        };

        private MainForm m_mainForm;

        public FilterForm()
        {
            InitializeComponent();
        }

        private void FilterForm_Load(object sender, EventArgs e)
        {
            m_mainForm = (MainForm)Owner;

            var dt = m_mainForm.DataTable;

            for (var i = 0; i < dt.Columns.Count; ++i)
                listBox2.Items.Add(dt.Columns[i].ColumnName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("Add filter(s) first!");
                return;
            }

            DataTable dt = m_mainForm.DataTable;

            if (m_filter == null)
                m_filter = dt;

            if (!checkBox1.Checked)
                m_filter = dt;

            var temp = dt.AsEnumerable().AsParallel().Where(Compare);

            if (temp.Count() != 0)
                m_filter = temp.CopyToDataTable();
            else
                m_filter = new DataTable();

            m_mainForm.SetDataSource(m_filter);
        }

        private void FilterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                Owner.Activate();
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dt = m_mainForm.DataTable;
            var colName = (string)listBox2.SelectedItem;
            var col = dt.Columns[colName];

            if (col.DataType == typeof(string))
                checkBox2.Visible = true;
            else
                checkBox2.Visible = false;

            comboBox3.Items.Clear();

            if (col.DataType == typeof(string))
                comboBox3.Items.AddRange(stringOperators);
            else if (col.DataType == typeof(float))
                comboBox3.Items.AddRange(floatOperators);
            else if (col.DataType == typeof(double))
                comboBox3.Items.AddRange(floatOperators);
            else if (col.DataType.IsPrimitive)
                comboBox3.Items.AddRange(decimalOperators);
            else
                MessageBox.Show("Unhandled type?");

            comboBox3.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Enter something first!");
                textBox2.Focus();
                return;
            }

            var fi = new FilterOptions((string)listBox2.SelectedItem, (ComparisonType)comboBox3.SelectedItem, textBox2.Text);

            var dt = m_mainForm.DataTable;
            var col = dt.Columns[fi.Column];

            try
            {
                if (col.DataType.IsPrimitive && col.DataType != typeof(float) && col.DataType != typeof(double))
                    if (fi.Value.StartsWith("0x", true, CultureInfo.InvariantCulture))
                        fi.Value = Convert.ToUInt64(fi.Value, 16).ToString(CultureInfo.InvariantCulture);

                Convert.ChangeType(fi.Value, col.DataType, CultureInfo.InvariantCulture);
            }
            catch
            {
                MessageBox.Show("Invalid filter!");
                return;
            }

            listBox1.Items.Add(fi);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                return;

            listBox1.Items.RemoveAt(listBox1.SelectedIndex);

            if (listBox1.Items.Count > 0)
                listBox1.SelectedIndex = 0;
        }

        public void ResetFilters()
        {
            listBox1.Items.Clear();
        }

        public void SetSelection(string column, string value)
        {
            for (var i = 0; i < listBox2.Items.Count; ++i)
            {
                if ((string)listBox2.Items[i] == column)
                {
                    listBox2.SelectedIndex = i;
                    break;
                }
            }

            textBox2.Text = value;
        }
    }
}
