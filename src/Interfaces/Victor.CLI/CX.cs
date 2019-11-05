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
            WriteInfoLine("Starting up, please wait...");
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
            Initialized = GeneralNLU.Initialized;
        }

        #endregion

        #region Properties
        public CXOptions Options { get; }

        protected SnipsNLUEngine GeneralNLU { get; }

        protected EDDIClient Client { get;  }
        
        protected JuliusSession JuliusSession { get; set; }

        protected bool InputEnabled { get; set; }

        public bool ASRReady { get; protected set; }

        public bool NLUDebug { get; protected set; }
        
        protected Tuple<DateTime, string> Context { get; set; }

        protected string[] Commands = { "help", "exit", "enable" };

        protected Dictionary<string, Action<object>> CommandHandlers { get; } = new Dictionary<string, Action<object>>();

        protected Dictionary<string, Action<int>> MenuHandlers { get; } = new Dictionary<string, Action<int>>();

        protected Dictionary<string, int> MenuIndexes { get; } = new Dictionary<string, int>();
        #endregion

        #region Methods

        #region UI and Input
        public void Start()
        {
            ThrowIfNotInitialized();
            if (!Options.Debug)
            {
                Sc.Clear();
            }
            Sc.Beep();
            Sc.Beep();
            WriteInfoLine("Welcome to Victor CX");
            SetContext("Welcome");
            Prompt();
        }

        public void SetContext(string c) => Context = new Tuple<DateTime, string>(DateTime.Now, c);

        public void HandleInput(DateTime time, string input)
        {
            ThrowIfNotInitialized();
            InputEnabled = false;
            if (Int32.TryParse(input, out int result) && Context.Item2.StartsWith("MENU"))
            {
                DispatchToMenuItem(Context.Item2, result);
            }
            else
            {
                var intents = GeneralNLU.GetIntents(input);
                if (intents.IsNone)
                {
                    WriteInfoLine("Sorry I don't know what you mean");
                }
                else if (intents.Top.Item2 < 0.5)
                {
                    WriteInfoLine("Sorry I'm not sure what you mean. Do you mean {0}?", intents.Top.Item1);
                }
                else
                {
                    if (NLUDebug)
                    {
                        WriteInfoLine("Intent: {0} Score: {1}.", intents.Top.Item1, intents.Top.Item2);
                        foreach (var e in intents.Entities)
                        {
                            WriteInfoLine("Entity:{0} Value:{1}.", e.Entity, e.Value.ValueValue);
                        }
                        if (intents.Top.Item1 == "enable")
                        {
                            Enable(intents);

                        }
                        else if (intents.Top.Item1 == "disable")
                        {
                            Disable(intents);
                        }
                    }
                    else
                    {
                        switch (intents.Top.Item1)
                        {
                            case "exit":
                                Exit();
                                break;
                            case "help":
                                Help(intents);
                                break;
                            case "hello":
                                Hello(intents);
                                break;
                            case "enable":
                                Enable(intents);
                                break;
                            case "disable":
                                Disable(intents);
                                break;
                            case "get bots":
                                GetBots();
                                break;
                            default:
                                break;
                        }
                    }
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
                    WriteInfoLine("Enter a number between 0 and {0}.", MenuIndexes[c]);
                    return;
                }
                else
                {
                    MenuHandlers[c].Invoke(i);
                }
            }
        }

        public void WriteInfoLine(string s) => Co.WriteLine(s, Color.PaleGoldenrod);

        public void WriteInfoLine(string template, params object[] args) => Co.WriteLineFormatted(template, Color.Pink, Color.PaleGoldenrod, args);

        public void WriteErrorLine(string s) => Co.WriteLine(s, Color.Red);

        public void WriteErrorLine(string template, params object[] args) => Co.WriteLineFormatted(template, Color.Pink, Color.Red, args);

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
            WriteInfoLine("Shutting down...");
            if (JuliusSession != null && JuliusSession.IsStarted)
            {
                JuliusSession.Stop();
            }
            Program.Exit(ExitResult.SUCCESS);
        }

        public void Hello(Intents intents)
        {
            var name = intents.Entities.Length > 0 ? intents.Entities.First().Value.ValueValue : "";
            WriteInfoLine("Hello {0} welcome to the Victor CX auditory user interface.", name);
        }

        public void Help(Intents intents)
        {
            var feature = intents.Entities.Length > 0 ? intents.Entities.First().RawValue : null;
            switch (feature)
            {
                case null:
                case "?":
                    WriteInfoLine("Victor CX is an auditory conversational user interface for interacting with an organisation\'s online services like product registration and on-boarding, product documentation, customer service and support.");
                    break;
                case "nlu":
                    WriteInfoLine("Victor CX uses natural language understading to understand a user's intent and the entities that are part of that intent.");
                    WriteInfoLine("You can enable NLU debug mode by entering {0}.", "enable debug");
                    break;
            }
            
        }

        public void Enable(Intents intents)
        {
            if (intents.Entities.Length == 0)
            {
                WriteErrorLine("Sorry I don't know what you want to enable.");
            }
            else
            {
                switch (intents.Entities.First().Value.ValueValue)
                {
                    case "debug":
                        if (!NLUDebug)
                        {
                            NLUDebug = true;
                            WriteInfoLine("NLU debug enabled. NLU information will be output and commands won't be executed ");
                            break;
                        }
                        else
                        {
                            WriteErrorLine("NLU debug is already enabled.");
                        }
                        break;
                    default:
                        WriteErrorLine("Sorry I don't know how to enable that.");
                        break;
                }
            }
        }

        public void Disable(Intents intents)
        {
            if (intents.Entities.Length == 0)
            {
                WriteErrorLine("Sorry I don't know what you want to disable.");
            }
            else
            {
                switch (intents.Entities.First().Value.ValueValue)
                {
                    case "debug":
                        if (NLUDebug)
                        {
                            NLUDebug = false;
                            WriteInfoLine("NLU debug disabled. Commands will be executed.");
                        }
                        else
                        {
                            WriteErrorLine("NLU debug is not enabled.");
                        }
                        break;
                    default:
                        WriteErrorLine("Sorry I don't know how to enable that.");
                        break;
                }
            }
        }

        public void EnableASR()
        {
            if (JuliusSession != null && JuliusSession.IsStarted)
            {
                throw new InvalidOperationException("Julius session already started.");
            }
            else if (JuliusSession != null && JuliusSession.Initialized && !JuliusSession.IsStarted)
            {
                JuliusSession.Start();
            }
            else
            {
                JuliusSession = new JuliusSession();
                JuliusSession.Recognized += JuliusSession_Recognized;
                if (JuliusSession.Initialized)
                {
                    WriteInfoLine("Automatic speech recognition enabled.");
                }
                else
                {
                    WriteErrorLine("Could not enable automatic speech recognition.");
                }

            }
        }
        public void GetBots()
        {
            WriteInfoLine("I'll check what bots are available on the Victor server.");
            var descriptors = Client.BotstoreBotsDescriptorsGetAsync(null, null, null).Result;
            if (descriptors == null)
            {
                WriteInfoLine("Sorry I couldn't get a proper response from the server.");
                return;
            }
            if (descriptors.Count == 0)
            {
                WriteInfoLine("Sorry the server says there are zero bots.");
                return;
            }
            WriteInfoLine("There are {0} bots on the server.", descriptors.Where(d => d.Description != "").Count());
            
            for(int i = 0; i < descriptors.Count; i++)
            {
                WriteInfoLine("{0}. {1}", i, descriptors.ElementAt(i).Name);
            }
            SetContext("MENU_BOTS");
        }

        public void GetBot(int i) 
        {
            WriteInfoLine("Get bot {0}.", i);
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
