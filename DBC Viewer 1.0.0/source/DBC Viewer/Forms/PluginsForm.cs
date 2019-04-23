using Microsoft.CSharp;
using PluginInterface;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DBCViewer
{
    public partial class PluginsForm : Form
    {
        public int PluginIndex { get; private set; }
        public Assembly NewPlugin { get; private set; }

        public PluginsForm()
        {
            InitializeComponent();
        }

        public void SetPlugins(IList<IPlugin> plugins)
        {
            foreach (IPlugin plugin in plugins)
            {
                var item = string.Format(CultureInfo.InvariantCulture, "{0}", plugin.GetType().Name);
                listBox1.Items.Add(item);
            }

            listBox1.SelectedIndex = 0;
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            PluginIndex = listBox1.SelectedIndex;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var form = new Form { FormBorderStyle = FormBorderStyle.SizableToolWindow, StartPosition = FormStartPosition.CenterParent, Width = 650, Height = 800 };
            form.Controls.Add(new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                Text = Properties.Resources.pluginTemplate,
                ScrollBars = ScrollBars.Both
            });
            form.ShowDialog();

            string sourceFile = form.Controls[0].Text;

            CSharpCodeProvider provider = new CSharpCodeProvider();

            // Build the parameters for source compilation.
            CompilerParameters cp = new CompilerParameters();

            // Add an assembly references.
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.ComponentModel.Composition.dll");
            cp.ReferencedAssemblies.Add("System.Data.dll");
            cp.ReferencedAssemblies.Add("System.Xml.dll");
            cp.ReferencedAssemblies.Add("PluginInterFace.dll");

            // Generate a class library instead of an executable.
            cp.GenerateExecutable = false;

            // Don't save the assembly as a physical file.
            cp.GenerateInMemory = true;

            // Invoke compilation.
            CompilerResults cr = provider.CompileAssemblyFromSource(cp, sourceFile);

            if (cr.Errors.Count > 0)
            {
                // Display compilation errors.
                StringBuilder sb = new StringBuilder();
                sb.AppendFormatLine("Errors building plugin {0}", "New Plugin");

                foreach (CompilerError ce in cr.Errors)
                {
                    sb.AppendFormatLine("    {0}", ce.ToString());
                }

                MessageBox.Show(sb.ToString());
            }
            else
            {
                //Console.WriteLine(string.Format("Source {0} built into {1} successfully.", "New Plugin", cr.PathToAssembly));

                NewPlugin = cr.CompiledAssembly;
            }

            // Return the results of compilation.
            if (cr.Errors.Count > 0)
            {
                PluginIndex = -1;
                DialogResult = DialogResult.Cancel;
                Close();
            }
            else
            {
                PluginIndex = listBox1.Items.Count;
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
