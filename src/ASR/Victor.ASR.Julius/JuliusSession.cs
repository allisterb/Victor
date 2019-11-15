using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Linq;

namespace Victor
{
    public delegate void RecognizedHandler(string sentence);

    public delegate void ListeningHandler();
    public class JuliusSession : Api
    {
        #region Constructors
        public JuliusSession(string juliusExe, string juliusConf, string args, CancellationToken ct) : base(ct)
        {
            JuliusExe = juliusExe ?? throw new ArgumentNullException("juliusExe");

            JuliusConf = juliusConf ?? throw new ArgumentNullException("juliusConf");

            JuliusArgs = new string[] { $"-C {JuliusConf}", args }; 

            JuliusProcess = new ConsoleProcess(JuliusExe, JuliusArgs.ToArray(), onOutput: OnProcessOutput, onError: OnErrorOutput);

            Initialized = JuliusProcess.Initialized;
        }

        public JuliusSession(CancellationToken ct) : this(Config("Julius:Exe").Replace('/', Path.DirectorySeparatorChar), 
            Config("Julius:MicConf").Replace('/', Path.DirectorySeparatorChar), Config("Julius:Args").Replace('/', Path.DirectorySeparatorChar), ct)
        { }
        
        public JuliusSession() : this(Cts.Token) {}
        #endregion

        #region Properties
        public string JuliusExe { get; }

        public string JuliusConf { get; }

        public IEnumerable<string> JuliusArgs { get; }

        public bool IsStarted { get; protected set; } = false;

        public bool IsListening { get; protected set; } = false;

        public bool IsPass1Recognizing { get; protected set; } = false;

        public bool IsPass1Complete { get; protected set; } = false;

        public string Pass1Text { get; protected set; }

        protected ConsoleProcess JuliusProcess { get; set; }

        protected Logger.Op Pass1RecognizingOp { get; set; }
        #endregion

        #region Methods
        public void Start()
        {
            if (IsStarted)
            {
                throw new InvalidOperationException("The Julius session is already started.");
            }
            JuliusProcess.Start();
            IsStarted = true;
        }

        public void Stop()
        {
            if (!IsStarted)
            {
                throw new InvalidOperationException("The Julius session is not started.");
            }
            JuliusProcess.Stop();
            IsStarted = false;
        }

        public void WaitForExit()
        {
            JuliusProcess.WaitForExit();
        }
        public void OnProcessOutput(string line)
        {
            if (!IsListening && line.Trim().Contains("So, the first input will not be recognized."))
            {
                IsListening = true;
                Listening?.Invoke();
            }
            else if (line.Trim().StartsWith("sentence1: <s> "))
            {
                IsPass1Recognizing = false;
                IsPass1Complete = true;
                Pass1Text = line.Replace("sentence1: <s> ", "").Replace("</s>", "").Trim();
                Debug("Recognized text: {0}", Pass1Text);
                Recognized?.Invoke(Pass1Text);
            }
            /*
            if (line.Trim().StartsWith("pass1_best"))
            {
                IsListening = false;
                IsPass1Recognizing = true;
                Pass1RecognizingOp = Begin("Recognizing");
            }
            else if (line.Trim().StartsWith("sentence1: <s> "))
            {
                Pass1RecognizingOp.Complete();
                IsPass1Recognizing = false;
                IsPass1Complete = true;
                Pass1Text = line.Replace("sentence1: <s> ", "").Replace("</s>", "").Trim();
                Info("Recognized text: {0}", Pass1Text);
                Info("Listening...");
            }*/

        }

        public void OnErrorOutput(string line)
        {
            if (line.ToLower().StartsWith("error:"))
            {
                Error(line);
            }

        }
        #endregion

        #region Events
        public event RecognizedHandler Recognized;

        public event ListeningHandler Listening;
        #endregion
    }
}
