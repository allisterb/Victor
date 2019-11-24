using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Victor.CLI
{
    public class CXHome : CUIPackage
    {
        #region Constructors
        public CXHome(CUIController controller) : base("Home", new SnipsNLUEngine(Path.Combine("Engines", "home")), controller)
        {
            MenuHandlers[Prefixed("PACKAGES")] = GetPackagesMenuItem;
            MenuIndexes[Prefixed("PACKAGES")] = 3;
            Initialized = NLUEngine.Initialized;
            if (!Initialized)
            {
                SayErrorLine("NLU engine for package {0} did not initialize. Exiting.", this.Name);
                Program.Exit(ExitResult.UNKNOWN_ERROR);
            }
        }
        #endregion

        #region Overriden members
        public override string[] VariableNames { get; } = { "NAME" };

        public override string[] MenuNames { get; } = { "PACKAGES" };

        public override string[] ItemNames { get; } = Array.Empty<string>();

        public override bool ParseIntent(CUIContext context, DateTime time, string input)
        {
            if (input.ToLower() == "vish")
            {
                GetPackagesMenuItem(1);
                return true;
            }
            else if (input.ToLower() == "services")
            {
                GetPackagesMenuItem(2);
                return true;
            }
            else if (input.ToLower() == "bots")
            {
                GetPackagesMenuItem(3);
                return true;
            }
            
            var intent = NLUEngine.GetIntent(input);
            if (Controller.DebugEnabled)
            {
                DebugIntent(intent);
            }
                      
            if (Empty(intent) || intent.Top.Score < 0.6)
            {
                return false;
            }
            else
            {
                switch (intent.Top.Label)
                {
                    case "help":
                        Help(intent);
                        break;
                    case "menu":
                        Menu(intent);
                        break;
                    case "exit":
                        Exit(intent);
                        break;
                    case "hello":
                        Hello(intent);
                        break;
                    case "enable":
                        Enable(intent);
                        break;
                    case "disable":
                        Disable(intent);
                        break;
                    case "back":
                        Back(intent);
                        break;
                    case "page":
                        Controller.ActivePackage.Page(intent);
                        break;
                    default:
                        break;
                }
                return true;
            }
        }
        
        #region Intents
        public override void Menu(Intent intent)
        {
            SetMenuContext("PACKAGES");
            SayInfoLine("Select a package to use.");
            SayInfoLine("1 {0}", "Vish");
            SayInfoLine("2 {0}", "Services");
            SayInfoLine("3 {0}", "Bots");
        }

        public override void Help(Intent intent)
        {
            var context = CurrentContext;
            if (ObjectEmpty(intent))
            {
                switch (context)
                {
                    case "WELCOME_HOME":
                        SayInfoLine("Welcome to Victor CX.");
                        SayInfoLine("Victor CX tasks and features are divided into packages. This is the {0} package which lets you jump to other packages or set global options and variables.", "HOME");
                        SayInfoLine("Say {0} to show the packages menu or {1} to get more background information. Say {2} to exit.", "menu", "info", "exit");
                        break;
                    case "MENU_HOME_PACKAGES":
                        SayInfoLine("Enter the number associated with the Victor CX package category you want to select.");
                        break;
                    default:
                        SayErrorLine("Unknown HOME context: {0}.", context);
                        SayInfoLine("Say {0} to enable debug mode.", "enable debug");
                        break;
                }
            }
            else
            {
                var (feature, package, function) = GetIntentFeaturePackageFunction(intent);
                
                if (!string.IsNullOrEmpty(feature))
                {
                    switch (feature)
                    {
                        case "this":
                            Help(null);
                            break;
                        case "nlu":
                            SayInfoLine("Victor CX uses natural language understanding to understand a user's intent and the entities that are part of that intent.");
                            SayInfoLine("A user does not have to enter an exact phrase or the exact command syntax but can express their intent using natural language and different phrases and synonymns.");
                            SayInfoLine("You can enable debug mode by entering {0} or {1}. For each user input this will print information about the intent and entities extracted by the NLU engine.", "enable debug", "debug on");
                            break;
                        case "asr":
                            SayInfoLine("Victor CX can use automatic speech recognition as an input method instead of or in addition to a keyboard or character input device.");
                            SayInfoLine("To enable ASR say {0} or {1}.", "asr on", "enable asr");
                            SayInfoLine("ASR quality may vary depending on your mic and environment. To test how ASR works with your hardware and environment run {0} from a command line.", "victor asr");
                            break;
                        case "package":
                            SayInfoLine("There are 3 package categories: {0}, {1} and {2}.", "Vish", "Services", "Bots.");
                            SayInfoLine("Vish is the Voice Interactive Shell with packages to help you manage and administer your computer or network or technology products like Red Hat OpenShift.");
                            SayInfoLine("Services let you access services like news and product information that do not require much interactivity.");
                            SayInfoLine("Bots are conversational agents that help you with tasks like filling out complex forms or completing complex multi-step processes and workflows that require a lot of interactivity.");
                            SayInfoLine("Use the {0} command to bring up the packages menu. You can also jump to a package category by entering the category name like {1}.\n", "menu", "vish");
                            break;
                        case "menu":
                            SayInfoLine("Menus provide an accessible way to break up or categorize a multi-step task. Enter the number corresponding to the task or category you would like to select next.");
                            break;
                        default:
                            SayInfoLine("No help so far for feature {0}.", feature);
                            break;
                    }
                }
                else if(!string.IsNullOrEmpty(package))
                {
                    switch(package)
                    {
                        case "vish":
                            SayInfoLine("Vish is the Voice Interactive Shell with packages to help you manage and administer your computer or network or technology products like Red Hat OpenShift.");
                            break;
                        case "services":
                            SayInfoLine("Services let you access services like news and product information that do not require much interactivity.");
                            break;
                        case "bots":
                            SayInfoLine("Bots are conversational agents that help you with tasks like filling out complex forms or completing complex multi-step processes and workflows that require a lot of interactivity.");
                            SayInfoLine("You can administer Victor CX bots by running {0} from the command-line.", "victor cui");
                            break;
                        default:
                            SayInfoLine("No help so far for package {0}.", package);
                            break;
                    }
                }
            }
        }

        public override void Info(Intent intent = null)
        {
            if (intent == null || intent.Entities.Length == 0)
            {
                SayInfoLine("Victor CX is an auditory conversational user interface for interacting with an organization's products and online customer services.");
                SayInfoLine("It is designed specifically for vision-impaired and differently-abled users and customers who are unable to effectively use a complex GUI or pointing devices like mice and touchscreens when interacting with applications and documents presented in a visual medium.");
            }
            else
            {
                var feature = intent.Entities.Length > 0 ? intent.Entities.FirstOrDefault(e => e.SlotName == "feature")?.Value : null;
                var package = intent.Entities.Length > 0 ? intent.Entities.FirstOrDefault(e => e.SlotName == "package")?.Value : null;
                var function = intent.Entities.Length > 0 ? intent.Entities.FirstOrDefault(e => e.SlotName == "function")?.Value : null;
                var context = Context.Count > 0 ? Context.Peek().Label : "";

                if (!string.IsNullOrEmpty(feature))
                {
                    feature = new string(feature.Where(c => Char.IsLetterOrDigit(c)).ToArray());
                    switch (feature)
                    {
                        case null:
                        case "":
                        case "this":
                            switch (context)
                            {
                                case "WELCOME_HOME":
                                    Info(null);
                                    break;
                                case "MENU_HOME_PACKAGES":
                                    Help(null);
                                    break;
                                default:
                                    SayErrorLine("Unknown home context: {0}.", context);
                                    break;
                            }
                            break;
                        case "nlu":
                            SayInfoLine("Victor CX uses natural language understanding to understand a user's intent and the entities that are part of that intent.");
                            SayInfoLine("A user does not have to enter an exact phrase or command but can express their intent using natural language and different phrases and synonymns.");
                            SayInfoLine("You can enable debug mode by entering {0} or {1}. For each user input this will print information about the intent and entities extracted by the NLU engine.", "enable debug", "debug on");
                            break;
                        default:
                            SayInfoLine("No info so far for {0}.", feature);
                            break;
                    }
                }
            }

        }
        #endregion

        #endregion

        #region Methods

        #region Intents

        public void Exit(Intent intent)
        {
            SayInfoLine("Shutting down...");
            if (Controller.ASREnabled)
            {
                Controller.StopASR();
            }
            Program.Exit(ExitResult.SUCCESS);
        }

        public void Hello(Intent intent)
        {
            var name = intent != null && intent.Entities.Length > 0 ? intent.Entities.First().Value : "";
            if (!string.IsNullOrEmpty(name))
            {
                SayInfoLine("Hello {0} welcome to the Victor CX auditory user interface.", name);
                Variables["HOME_NAME"] = name;
            }
            else if (Variables["HOME_NAME"] != null)
            {
                SayInfoLine("Hello {0} welcome to the Victor CX auditory user interface.", Variables["HOME_NAME"]);
            }
            else
            {
                SayInfoLine("Hello, welcome to the Victor CX auditory user interface.");
            }
            SayInfoLine("Enter {0} to see a menu of options or {1} to get help. Enter {2} if you want to quit.", "menu", "help", "exit");
        }

        protected void Enable(Intent intent)
        {
            if (intent.Entities.Length == 0)
            {
                SayErrorLine("Sorry I don't know what you want to enable.");
            }
            else
            {
                switch (intent.Entities.First().Value)
                {
                    case "debug":
                        if (!Controller.DebugEnabled)
                        {
                            Controller.DebugEnabled = true;
                            SayInfoLine("Debug enabled.");
                            break;
                        }
                        else
                        {
                            SayErrorLine("Debug is already enabled.");
                        }
                        break;
                    case "asr":
                        if (!Controller.ASREnabled)
                        {
                            Controller.EnableASR();
                            break;
                        }
                        else
                        {
                            SayErrorLine("ASR is already enabled.");
                        }
                        break;
                    default:
                        SayErrorLine("Sorry I don't know how to enable that.");
                        break;
                }
            }
        }

        protected void Disable(Intent intent)
        {
            if (intent.Entities.Length == 0)
            {
                SayErrorLine("Sorry I don't know what you want to disable.");
            }
            else
            {
                switch (intent.Entities.First().Value)
                {
                    case "debug":
                        if (Controller.DebugEnabled)
                        {
                            Controller.DebugEnabled = false;
                            SayInfoLine("Debug disabled. Commands will be executed.");
                        }
                        else
                        {
                            SayErrorLine("Debug is not enabled.");
                        }
                        break;
                    case "asr":
                        if (Controller.ASREnabled)
                        {
                            Controller.StopASR();
                            SayInfoLine("ASR disabled.");
                        }
                        else
                        {
                            SayErrorLine("ASR is not enabled.");
                        }
                        break;
                    default:
                        SayErrorLine("Sorry I don't know how to enable that.");
                        break;
                }
            }
        }

        public void Back(Intent intent)
        {
            if (Controller.ActivePackage != Controller.PreviousPackage)
            {
                Controller.ActivePackage = Controller.PreviousPackage;
                Controller.ActivePackage.DispatchIntent(null, Controller.ActivePackage.Menu);
            }
            else
            {
                Controller.Buzz();
            }
        }
        #endregion

        protected void GetPackagesMenuItem(int i)
        {
            switch(i - 1)
            {
                case 0:
                    if (!SubPackages.Any(p => p.Name == "Vish"))
                    {
                        SayInfoLine("Loading Vish package...");
                        Controller.StartBeeper();
                        SubPackages.Add(new Vish(this.Controller));
                        Controller.StopBeeper();
                    }
                    Controller.SetActivePackage(SubPackages.Single(p => p.Name == "Vish"));
                    DispatchIntent(null, Controller.ActivePackage.Menu);
                    break;
                case 2:
                    LoadBots();
                    break;

                default:
                    throw new IndexOutOfRangeException();
            }
        }

        protected void LoadBots()
        {
            if (!SubPackages.Any(p => p.Name == "Bots"))
            {
                SayInfoLine("Loading Bots...");
                Controller.StartBeeper();
                var bots = new Bots(this.Controller);
                Controller.StopBeeper();
                if (bots.Initialized)
                {
                    SubPackages.Add(new Bots(this.Controller));
                }
                else
                {
                    SayErrorLine("The Bots package failed to initialize.");
                    return;
                }
                
            }
            Controller.ActivePackage = SubPackages.Single(p => p.Name == "Bots"); 

            if (CurrentContext.StartsWith("MENU_"))
            {
                DispatchIntent(null, Controller.ActivePackage.Menu);
            }
            else
            {
                DispatchIntent(null, Controller.ActivePackage.Welcome);
            }

        }
        #endregion
    }
}
