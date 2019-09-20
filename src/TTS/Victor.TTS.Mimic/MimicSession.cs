using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Linq;

namespace Victor
{
    public class MimicSession : Api
    {
        #region Constructors
        public MimicSession(string mimicExe, string mimicText, string args, CancellationToken ct) : base(ct)
        {
            MimicExe = mimicExe ?? throw new ArgumentNullException("mimicExe");

            MimicText = mimicText ?? throw new ArgumentNullException("mimicText");

            MimicArgs = new string[] { $"-v -t \"{MimicText}\"", args };

            MimicProcess = new ConsoleProcess(MimicExe, MimicArgs.ToArray(), onOutput: OnProcessOutput, onError: OnErrorOutput);

            Initialized = MimicProcess.Initialized;
        }

        public MimicSession(string mimicText, CancellationToken ct) : this(Config("Mimic:Exe").Replace('/', Path.DirectorySeparatorChar),
            mimicText, Config("Mimic:Args").Replace('/', Path.DirectorySeparatorChar), ct)
        { }

        public MimicSession(string text) : this(text, Cts.Token) { }
        #endregion

        #region Properties
        public string MimicExe { get; }

        public string MimicText { get; }

        public IEnumerable<string> MimicArgs { get; }

        protected ConsoleProcess MimicProcess { get; set; }

        #endregion

        #region Methods
        public void Run()
        {
            MimicProcess.Start();
            MimicProcess.WaitForExit();
        }

        public void OnProcessOutput(string line)
        {
            if (line.StartsWith("If audio works"))
            {
                return;
            }
            else
            {
                Info("Mimic TTS: {0}", line);
            }
        }

        public void OnErrorOutput(string line)
        {
        }
        #endregion

        #region Events
        
        #endregion
    }
}
