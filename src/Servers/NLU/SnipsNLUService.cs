using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Victor.Server.NLU
{
    public class SnipsNLUService : Api
    {
        public SnipsNLUService(string engineDirectory) : base()
        {
            EnginesDirectory = new DirectoryInfo(engineDirectory);
            if (!EnginesDirectory.Exists)
            {
                return;
            }
            var dirs = EnginesDirectory.EnumerateDirectories();
            foreach(var d in dirs)
            {
                Engines.Add(d.Name, new SnipsNLUEngine(d.FullName, Api.Ct));
            }
        }

        public DirectoryInfo EnginesDirectory { get; } 
        
        public Dictionary<string, SnipsNLUEngine> Engines { get; } = new Dictionary<string, SnipsNLUEngine>();
    }
}
