using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;


using Sc = System.Console;

using Colorful;
using Co = Colorful.Console;

namespace Victor.CLI
{
    public class CX : Api
    {
        #region Constructors
        public CX(CXOptions options) : base() 
        {
            Options = options;
            Client = new EDDIClient("http://eddi-evals25-shared-7daa.apps.hackathon.rhmi.io", HttpClient);
            GeneralNLU = new SnipsNLUEngine(Path.Combine("Engines", "general"));
            JuliusSession = new JuliusSession();
            JuliusSession.Recognized += JuliusSession_Recognized;
            Initialized = GeneralNLU.Initialized && JuliusSession.Initialized;
        }

        #endregion

        #region Properties
        public CXOptions Options { get; }
        protected EDDIClient Client { get;  }
        protected JuliusSession JuliusSession { get; }

        protected SnipsNLUEngine GeneralNLU { get; }

        protected Tuple<DateTime, string> Context { get; set; }

        protected bool InputEnabled { get; set; }

        public void SetContext(string c) => Context = new Tuple<DateTime, string>(DateTime.Now, c);
        #endregion

        #region Methods
        public void Start()
        {
            ThrowIfNotInitialized();
            Sc.Beep();
            Sc.Beep();
            
            WriteLine("Welcome to Victor CX");
            SetContext("Welcome");
            JuliusSession.Start();
            Prompt();
            JuliusSession.Stop();
        }

        public void HandleInput(DateTime time, string input)
        {
            InputEnabled = false;
            
            GeneralNLU.GetIntents(input, out string[] intents, out string json, out string error);
            Debug("Intents:{0}", json);
            switch (input)
            {
                case "exit":
                    JuliusSession.Stop();
                    Program.Exit(ExitResult.SUCCESS);
                    break;
                case "help":
                    WriteLine("Help");
                    break;
                default:
                    WriteLine("Sorry I don't know what you mean");
                    break;
            }
            Prompt();
        }

        public void WriteLine(string s) => Co.WriteLine(s, Color.PaleGoldenrod);

        public void Prompt()
        {
            InputEnabled = true;
            string i = ReadLine.Read("|> ");
            HandleInput(DateTime.Now, i);
        }

        #endregion

        #region Event Handlers
        private void JuliusSession_Recognized(string sentence)
        {
            if (InputEnabled)
            {
                ReadLine.Send(sentence);
                ReadLine.Send(ConsoleKey.Enter);
            }
        }
        #endregion
    }
}
