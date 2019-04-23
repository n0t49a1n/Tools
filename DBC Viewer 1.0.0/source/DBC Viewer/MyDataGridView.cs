using System.Windows.Forms;

namespace DBCViewer
{
    public class MyDataGridView : DataGridView
    {
        public MyDataGridView()
            : base()
        {
            DoubleBuffered = true;
        }
    }
}
