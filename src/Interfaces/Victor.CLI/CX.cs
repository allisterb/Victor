using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
            Client = new EDDIClient(Config("CUI_EDDI_SERVER_URL"), HttpClient);
            GeneralNLU = new SnipsNLUEngine(Path.Combine("Engines", "general"));
            ServicesNLU = new SnipsNLUEngine(Path.Combine("Engines", "services"));
            Initialized = GeneralNLU.Initialized && ServicesNLU.Initialized;
        }

        #endregion

        #region Properties
        public CXOptions Options { get; }

        protected SnipsNLUEngine GeneralNLU { get; }

        protected SnipsNLUEngine ServicesNLU { get; }

        protected EDDIClient Client { get;  }
        
        protected JuliusSession JuliusSession { get; set; }

        protected bool InputEnabled { get; set; }

        public bool ASRReady { get; protected set; }

        public bool NLUDebug { get; protected set; }

        public bool BeeperOn { get; protected set; }

        protected Stack<Tuple<DateTime, string>> Context { get; } = new Stack<Tuple<DateTime, string>>();

        protected Dictionary<string, Action<object>> CommandHandlers { get; } = new Dictionary<string, Action<object>>();

        protected Dictionary<string, Action<int>> MenuHandlers { get; } = new Dictionary<string, Action<int>>();

        protected Dictionary<string, int> MenuIndexes { get; } = new Dictionary<string, int>();

        protected Dictionary<string, List<object>> MenuItems { get; } = new Dictionary<string, List<object>>();

        protected Dictionary<string, int> SelectedMenuItem = new Dictionary<string, int>();
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
            
            WriteInfoLine("Welcome to Victor CX");
            SetContext("WELCOME");
            ReadLine.HistoryEnabled = true;
            if (Program.beeperOn) Program.StopBeeper();
            Prompt();
        }

        public void SetContext(string c) => Context.Push(new Tuple<DateTime, string>(DateTime.Now, c));

        public void HandleInput(DateTime time, string input)
        {
            ThrowIfNotInitialized();
            InputEnabled = false;
            if (Int32.TryParse(input, out int result) && Context.Peek().Item2.StartsWith("MENU"))
            {
                DispatchToMenuItem(Context.Peek().Item2, result);
            }
            else
            {
                var intents = ServicesNLU.GetSnipsIntents(input);
                if (!intents.IsNone && intents.Top.Item2 >= 0.8)
                {
                    if (NLUDebug)
                    {
                        WriteInfoLine("Context: {0}, Category: {1}, Intent: {2} Score: {3}.", Context.PeekIfNotEmpty(), "Bots", intents.Top.Item1, intents.Top.Item2);
                        foreach (var e in intents.Entities)
                        {
                            WriteInfoLine("Entity:{0} Value:{1}.", e.Entity, e.Value.ValueValue);
                        }

                    }
                    else 
                    {
                        Services(intents);
                    }
                }  
                else
                {
                    intents = GeneralNLU.GetSnipsIntents(input);
                    if (intents.IsNone)
                    {
                        WriteInfoLine("Sorry I don't know what you mean. Type {0} to get a list of things you can do.", "help");
                    }
                    else if (intents.Top.Item2 < 0.6)
                    {
                        WriteInfoLine("Sorry I'm not sure what you mean. Do you mean {0}?", intents.Top.Item1);
                    }
                    else
                    {
                        if (NLUDebug)
                        {
                            WriteInfoLine("Context: {0}, Category: {1}, Intent: {2} Score: {3}.", Context.PeekIfNotEmpty(), "Bots", intents.Top.Item1, intents.Top.Item2);
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
                            else if (intents.Top.Item1 == "exit")
                            {
                                Exit();
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
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            Prompt();
        }

        public void DispatchToMenuItem(string c, int i)
        {            
            if (i < 0 || i > MenuIndexes[c])
            {
                WriteInfoLine("Enter a number between {0} and {1}.", 1, MenuIndexes[c]);
                return;
            }
            else
            {
                Context.Pop();
                MenuHandlers[c].Invoke(i);
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
        #endregion

        #region Features

        #region General
        public void Exit()
        {
            WriteInfoLine("Shutting down...");
            if (JuliusSession != null && JuliusSession.IsStarted)
            {
                JuliusSession.Stop();
            }
            Program.Exit(ExitResult.SUCCESS);
        }

        public void Hello(SnipsIntents intents)
        {
            var name = intents.Entities.Length > 0 ? intents.Entities.First().Value.ValueValue : "";
            WriteInfoLine("Hello {0} welcome to the Victor CX auditory user interface.", name);
        }

        public void Help(SnipsIntents intents)
        {
            var feature = intents.Entities.Length > 0 ? intents.Entities.First().RawValue : null;
            if (!string.IsNullOrEmpty(feature))
            {
                feature = new string(feature.Where(c => Char.IsLetterOrDigit(c)).ToArray());
            }
            var context = Context.Count > 0 ? Context.Peek().Item2 : string.Empty;
            switch (feature)
            {
                case null:
                case "":
                case "this":
                    switch (context)
                    {
                        case "WELCOME":
                            WriteInfoLine("Victor CX is an auditory conversational user interface for interacting with an organisation\'s online services like product registration and on-boarding, product documentation, customer service and support.");
                            break;
                        case "MENU_BOTS":
                            WriteInfoLine("Enter a number between {0} and {1} to select the bot you want to talk to.", 1, MenuIndexes["MENU_BOTS"]);
                            break;
                        case "ITEM_BOTS":
                            WriteInfoLine("Say {0} to talk to this bot, {1} to describe this bot, {2} back to go back to the previous menU.", "chat", "about", "back");
                            break;
                        default:
                            break;
                    }
                    break;
                case "nlu":
                    WriteInfoLine("Victor CX uses natural language understading to understand a user's intent and the entities that are part of that intent.");
                    WriteInfoLine("You can enable NLU debug mode by entering {0}.", "enable debug");
                    break;
                case "bots":
                    WriteInfoLine("Bots provide different services. Say {0} to see the bots available.");
                    break;
            }
            
        }

        public void Enable(SnipsIntents intents)
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
                            WriteInfoLine("NLU debug enabled. NLU information will be output and commands won't be executed.");
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

        public void Disable(SnipsIntents intents)
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
        #endregion

        #region Bots
        public void Services(SnipsIntents intents)
        {
            switch(intents.Top.Item1)
            {
                case "bot_info":
                    if (intents.Entities.Length == 0)
                    {
                        WriteInfoLine("I'll check what bots are available on the Victor server.");
                        Program.StartBeeper();
                        var descriptors = Client.BotstoreBotsDescriptorsGetAsync(null, null, null).Result;
                        Program.StopBeeper();
                        if (descriptors == null)
                        {
                            WriteErrorLine("Sorry I couldn't get a proper response from the server.");
                        }
                        else if (descriptors.Count == 0)
                        {
                            WriteInfoLine("Sorry the server says there are zero bots.");
                        }
                        else
                        {
                            WriteInfoLine("There are {0} bots on the server.", descriptors.Count());

                            for (int i = 1; i <= descriptors.Count; i++)
                            {
                                WriteInfoLine("{0}. {1}", i, descriptors.ElementAt(i - 1).Name);
                            }
                            WriteInfoLine("You can now enter the number of the bot you want talk to.");
                            Context.PopIfNotEmpty();
                            MenuIndexes["MENU_BOTS"] = descriptors.Count;
                            MenuItems["MENU_BOTS"] = descriptors.Cast<object>().ToList();
                            SetContext("MENU_BOTS");
                        }
                    }
                    else
                    {
                        WriteInfoLine("bot info {0}.", intents.Entities.First().Value.ValueValue);
                    }
                    break;
                case "bot_help":
                    if (intents.Entities.Length == 0)
                    {
                        WriteInfoLine("bot help.");
                    }
                    else
                    {
                        WriteInfoLine("bot help {0}.", intents.Entities.First().Value.ValueValue);
                    }
                    break;
                case "vish_info":
                    if (intents.Entities.Length == 0)
                    {
                        WriteInfoLine("vish info.");
                    }
                    else
                    {
                        WriteInfoLine("info {0}.", intents.Entities.First().Value.ValueValue);
                    }
                    break;
                case "vish_help":
                    if (intents.Entities.Length == 0)
                    {
                        WriteInfoLine("vish help.");
                    }
                    else
                    {
                        WriteInfoLine(" help {0}.", intents.Entities.First().Value.ValueValue);
                    }
                    break;
                default:
                    break;
            }
        }

        public void Vish(SnipsIntents intents)
        {
            WriteInfoLine("Vish");
        }

        public void GetBot(int i)
        {
            var bots = MenuItems["MENU_BOTS"].Cast<DocumentDescriptor>().ToList();
            var bot = bots[i - 1];
            SelectedMenuItem["MENU_BOTS"] = i;
            SetContext("ITEM_BOTS");
            WriteInfoLine("Bot {0}: {1}.", i, bot.Description);
        }

        public void DescribeCurrentBot()
        {
            if (Context.Peek().Item2 != "ITEM_BOTS")
            {
                WriteErrorLine("Sorry I don't know what you are doing.");
                return;
            }
            var bots = MenuItems["MENU_BOTS"].Cast<DocumentDescriptor>().ToList();
            var bot = bots.ElementAt(SelectedMenuItem["MENU_BOTS"]);
            WriteInfoLine("Name: {0}: Description: {1} Id: {2}.", bot.Name, bot.Description, bot.ResourceId);
        }
        #endregion

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
