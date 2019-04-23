using PluginInterface;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace DBCViewer
{
    public partial class DefinitionCatalog : Form
    {
        private int DefinitionIndex;

        public DefinitionCatalog()
        {
            InitializeComponent();
        }

        public static DBFilesClient SelectCatalog(string path)
        {
            using (var selector = new DefinitionCatalog())
            {
                var defpath = Path.Combine(path, "definitions");

                if (!Directory.Exists(defpath))
                    Directory.CreateDirectory(defpath);

                var files = Directory.GetFiles(defpath, "*.xml");

                if (files.Length == 0)
                {
                    selector.DialogResult = DialogResult.OK;
                    selector.Close();
                    return new DBFilesClient() { File = Path.Combine(defpath, "default.xml"), Tables = new List<Table>() };
                }

                foreach (var file in files)
                    selector.listBox1.Items.Add(Path.GetFileName(file));

                selector.ShowDialog();

                if (selector.DefinitionIndex != -1)
                    return DBFilesClient.Load(files[selector.DefinitionIndex]);

                return null;
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DefinitionIndex = listBox1.IndexFromPoint(e.Location);
            if (DefinitionIndex == -1)
                return;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
