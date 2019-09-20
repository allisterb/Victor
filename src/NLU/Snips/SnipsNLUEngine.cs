using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Victor.SnipsNLU;

namespace Victor
{
    public class SnipsNLUEngine: Api
    {
        #region Constructors
        public SnipsNLUEngine(string engineDir, CancellationToken ct) : base(ct)
        {
            EngineDir = Path.Combine(AssemblyDirectory.FullName, engineDir);
            if (!Directory.Exists(EngineDir))
            {
                Error("The directory {0} does not exist.", EngineDir);
            }
            if (SnipsApi.CreateEngineFromDir(EngineDir, out IntPtr enginePtr))
            {
                EnginePtr = enginePtr;
                Initialized = true;
                Info("Created SnipsNLU engine from directory {0}", EngineDir);
            }
        }
        #endregion

        #region Properties
        public SnipsNLUEngine(CancellationToken ct) : this(Config("SnipsNLU:EngineDir"), ct) {}

        public SnipsNLUEngine() : this(Cts.Token) { }

        public IntPtr EnginePtr { get; } 

        public string EngineDir { get; }
        #endregion

        #region Methods
        public void GetIntents(string input, out string intent, out string json, out string error)
        {
            ThrowIfNotInitialized();
            error = "";
            intent = "";
            json = "";
            if (SnipsApi.GetIntents(EnginePtr, input, out libsnips.CIntentClassifierResult[] results))
            {
                SnipsApi.GetSlotsIntoJson(EnginePtr, input, results.First().intent_name, out json, out error);
            }
        }
        #endregion
    }
}
