using PluginInterface;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace DBCViewer
{
    public partial class DefinitionSelect : Form
    {
        public int DefinitionIndex { get; private set; }

        public DefinitionSelect()
        {
            InitializeComponent();
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = listBox1.IndexFromPoint(e.Location);
            if (index == -1)
                return;
            DefinitionIndex = index;
            DialogResult = DialogResult.OK;
            Close();
        }

        public void SetDefinitions(IEnumerable<Table> definitions)
        {
            foreach (Table def in definitions)
            {
                var item = string.Format(CultureInfo.InvariantCulture, "{0} (build {1})", def.Name, def.Build);
                listBox1.Items.Add(item);
            }

            listBox1.SelectedIndex = 0;
        }
    }
}
