using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
            WriteLine("Starting up, please wait...");
            MenuHandlers = new Dictionary<string, Action<int>>()
            {
                { "MENU_BOTS", (i) => GetBot(i) }
            };
            MenuIndexes = new Dictionary<string, int>()
            {
                {"MENU_BOTS", 0 }
            };
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

        public bool ASRReady { get; protected set; }

        protected SnipsNLUEngine GeneralNLU { get; }

        protected Tuple<DateTime, string> Context { get; set; }

        protected bool InputEnabled { get; set; }

        protected string[] Commands = { "help", "exit" };
        protected Dictionary<string, Action<int>> MenuHandlers { get; } = new Dictionary<string, Action<int>>();

        protected Dictionary<string, int> MenuIndexes { get; } = new Dictionary<string, int>();
        #endregion

        #region Methods

        #region UI
        public void Start()
        {
            ThrowIfNotInitialized();
            if (!Options.Debug)
            {
                Sc.Clear();
            }
            Sc.Beep();
            Sc.Beep();
            WriteLine("Welcome to Victor CX");
            SetContext("Welcome");
            JuliusSession.Start();
            Prompt();
            JuliusSession.Stop();
        }

        public void SetContext(string c) => Context = new Tuple<DateTime, string>(DateTime.Now, c);

        public void HandleInput(DateTime time, string input)
        {
            InputEnabled = false;
            if (Int32.TryParse(input, out int result) && Context.Item2.StartsWith("MENU"))
            {
                DispatchToMenuItem(Context.Item2, result);
            }
            else
            {
                GeneralNLU.GetIntents(input, out string[] intents, out string json, out string error);
                Debug("Intents:{0}", json);
                switch (input)
                {
                    case "exit":
                        Exit();
                        break;
                    case "help":
                        Help();
                        break;
                    case "get bots":
                        GetBots();
                        break;
                    default:
                        WriteLine("Sorry I don't know what you mean");
                        break;
                }
            }
            Prompt();
        }

        public void DispatchToMenuItem(string c, int i)
        {
            if (MenuHandlers.ContainsKey(c))
            {
                if (i < 0 || i > MenuIndexes[c])
                {
                    WriteLine("Enter a number between 0 and {0}.", MenuIndexes[c]);
                    return;
                }
                else
                {
                    MenuHandlers[c].Invoke(i);
                }
            }
        }

        public void WriteLine(string s) => Co.WriteLine(s, Color.PaleGoldenrod);

        public void WriteLine(string template, params object[] args) => Co.WriteLine(template, Color.PaleGoldenrod, args);

        public void Prompt()
        {
            InputEnabled = true;
            string i = ReadLine.Read("|> ");
            HandleInput(DateTime.Now, i);
        }
        #endregion

        #region Features
        public void Exit()
        {
            WriteLine("Shutting down...");
            JuliusSession.Stop();
            Program.Exit(ExitResult.SUCCESS);
        }
        public void Help()
        {
            WriteLine("VictorCX is an auditory user interface for interacting with an organisation\'s online services like product registration and on-boarding, product documentation, customer service and support."); 
        }

        public void GetBots()
        {
            WriteLine("I'll check what bots are available on the Victor server.");
            var descriptors = Client.BotstoreBotsDescriptorsGetAsync(null, null, null).Result;
            if (descriptors == null)
            {
                WriteLine("Sorry I couldn't get a proper response from the server.");
                return;
            }
            if (descriptors.Count == 0)
            {
                WriteLine("Sorry the server says there are zero bots.");
                return;
            }
            WriteLine("There are {0} bots on the server.", descriptors.Where(d => d.Description != "").Count());
            
            for(int i = 0; i < descriptors.Count; i++)
            {
                WriteLine("{0}. {1}", i, descriptors.ElementAt(i).Name);
            }
            SetContext("MENU_BOTS");
        }

        public void GetBot(int i) 
        {
            WriteLine("Get bot {0}.", i);
        }
        #endregion

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
