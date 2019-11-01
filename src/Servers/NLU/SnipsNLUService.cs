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
                var e = new SnipsNLUEngine(d.FullName, Api.Ct);
                if (e.Initialized)
                {
                    Engines.Add(d.Name, e);
                }
     
            }
            if (Engines.Count == 0)
            {
                Error("Did not initialize any Snips NLU engines from directory {0}", engineDirectory);
                return;
            }
            else
            {
                Initialized = true;
                Info("Initialized {0} Snips NLU engines: {1}.", Engines.Count, Engines.Keys);
            }
        }

        public DirectoryInfo EnginesDirectory { get; } 
        
        public Dictionary<string, SnipsNLUEngine> Engines { get; } = new Dictionary<string, SnipsNLUEngine>();
    }
}
