using System;
using System.Data;

namespace PluginInterface
{
    public interface IPlugin
    {
        // main plugin method
        void Run(DataTable table);
        // main plugin method #2
        void Run(IClientDBReader table);
        // callback to main program
        Action<int> Finished { get; set; }
        Action ClearDataTable { get; set; }
    }
}
