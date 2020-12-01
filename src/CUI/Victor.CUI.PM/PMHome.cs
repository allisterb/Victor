using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Victor.CUI
{
    public class PMHome : CUIPackage
    {
        #region Constructors
        public PMHome(CUIController controller) : base("ProjectManagement", new SnipsNLUEngine(Path.Combine("Engines", "PM")), controller)
        {
            MenuHandlers[Prefixed("FEATURES")] = GetFeaturesMenuItem;
            MenuIndexes[Prefixed("FEATURES")] = 3;
            Initialized = NLUEngine.Initialized;
            if (!Initialized)
            {
                SayErrorLine("NLU engine for package {0} did not initialize. Exiting.", this.Name);
                Controller.Exit(ExitResult.UNKNOWN_ERROR);
            }
        }
        #endregion
        
        #region Overriden members
        public override string[] VariableNames { get; } = { "BOARD" };

        public override string[] MenuNames { get; } = { "FEATURES" };

        public override string[] ItemNames { get; } = Array.Empty<string>();

        public override bool ParseIntent(CUIContext context, DateTime time, string input)
        {
            switch(input.ToLower())
            {
                case "help":
                    Help(null);
                    return true;
                case "menu":
                    Menu(null);
                    return true;
                case "exit":
                    Exit(null);
                    return true;
                case "hello":
                    Hello(null);
                    return true;
                case "enable":
                    Enable(null);
                    return true;
                case "disable":
                    Disable(null);
                    return true;
                case "back":
                    Back(null);
                    return true;
                case "page":
                    Controller.ActivePackage.Page(null);
                    return true;
            }
            var intent = NLUEngine.GetIntent(input);
            if (Controller.DebugEnabled)
            {
                DebugIntent(intent);
            }                      
            if (Empty(intent) || intent.Top.Score < 0.7)
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
            SetMenuContext("FEATURES");
            SayInfoLine("Select a feature to use.");
            SayInfoLine("1. {0}", "Boards");
            SayInfoLine("2. {0}", "Tasks");
            SayInfoLine("3. {0}", "Users");
        }

        public override void Help(Intent intent)
        {
            var context = CurrentContext;
            if (ObjectEmpty(intent))
            {
                switch (context)
                {
                    case "WELCOME_PROJECTMANAGEMENT":
                        SayInfoLine("Welcome to Victor PM.");
                        SayInfoLine("Victor PM is a project management program designed for people with vision or other disabilities.");
                        SayInfoLine("Victor PM is a project management program designed for people with vision or other disabilities.");
                        SayInfoLine("Victor SM tasks and features are divided into packages. This is the {0} package which lets you jump to other packages or set global options and variables.", "HOME");
                        SayInfoLine("Say {0} to show the main menu or {1} to get more background information. Say {2} to exit.", "menu", "info", "exit");
                        break;
                    case "MENU_HOME_PACKAGES":
                        SayInfoLine("Enter the number associated with the Victor SM package category you want to select.");
                        break;
                    default:
                        SayErrorLine("Unknown HELP context: {0}.", context);
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
                            SayInfoLine("Victor SM uses natural language understanding to understand a user's intent and the entities that are part of that intent.");
                            SayInfoLine("A user does not have to enter an exact phrase or the exact command syntax but can express their intent using natural language and different phrases and synonymns.");
                            SayInfoLine("You can enable debug mode by entering {0} or {1}. For each user input this will print information about the intent and entities extracted by the NLU engine.", "enable debug", "debug on");
                            break;
                        case "asr":
                            SayInfoLine("Victor SM can use automatic speech recognition as an input method instead of or in addition to a keyboard or character input device.");
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
                        case "bot":
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
                SayInfoLine("Victor SM is an auditory conversational user interface for interacting with an organization's products and online customer services.");
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
            Controller.Exit(ExitResult.SUCCESS);
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

        protected void GetFeaturesMenuItem(int i)
        {
            
            switch(i - 1)
            {
                case 0:
                    LoadBoards();
                    break;

                default:
                    throw new IndexOutOfRangeException();
            }
        }

        protected void LoadBoards()
        {
            

        }

        protected void LoadUsers()
        {

        }
        #endregion
    }
}
