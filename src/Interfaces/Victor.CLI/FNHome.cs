﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Victor.CUI;
namespace Victor.CLI
{
    public class FNHome : Package
    {
        #region Constructors
        public FNHome(Controller controller) : base("Home", new SnipsNLUEngine(Path.Combine("Engines", "fn")), controller)
        {
            //MenuHandlers[Prefixed("PACKAGES")] = GetPackagesMenuItem;
            //MenuIndexes[Prefixed("PACKAGES")] = 3;
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

        public override bool ParseIntent(Context context, DateTime time, string input)
        {
            switch (input.ToLower())
            {
                case "info":
                    Info(null);
                    return true;
                case "help":
                    Help(null);
                    return true;
                case "menu":
                    Menu(null);
                    return true;
                case "exit":
                    Exit(null);
                    return true;

                case "enable asr":
                    this.Controller.EnableASR();
                    return true;
                case "disable asr":
                    this.Controller.StopASR();
                    return true;
                case "back":
                    Back(null);
                    return true;
                case "page":
                    Page(null);
                    return true;
                    //case "list boards":
                    //DispatchIntent(intent, List);
                    //break;
                    //var boards = PM.MdcApi.GetBoards();
                    //for(int i = 0; i < boards.Result.Boards.Count; i++)
                    //{
                    //    SayInfoLine("{0}.{1}", i, boards.Result.Boards[i].Name);
                    //}
                    //return true;
                    //foreach(var b in broads)
                    //{
                    //    SayInfoLine("")
                    //}
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
                        Page(intent);
                        break;
                    default:
                        break;
                }
                return true;
            }
        }
        
        #region Intents
        protected override void Menu(Intent intent)
        {
            SetMenuContext("PACKAGES");
            SayInfoLine("Select a package to use.");
            SayInfoLine("1. {0}", "Medications");
            SayInfoLine("2. {0}", "Symptoms");
            SayInfoLine("3. {0}", "Activities");
            SayInfoLine("4. {0}", "Evaluations");
            SayInfoLine("5. {0}", "Knowledge");
        }

        protected override void Help(Intent intent)
        {
            var context = CurrentContext;
            if (EmptyEntities(intent))
            {
                switch (context)
                {
                    case "WELCOME_HOME":
                        SayInfoLine("Welcome to Victor SM.");
                        SayInfoLine("Victor SM tasks and features are divided into packages. This is the {0} package which lets you jump to other packages or set global options and variables.", "HOME");
                        SayInfoLine("Say {0} to show the packages menu or {1} to get more background information. Say {2} to exit.", "menu", "info", "exit");
                        break;
                    case "MENU_HOME_PACKAGES":
                        SayInfoLine("Enter the number associated with the Victor SM package category you want to select.");
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

        protected override void Info(Intent intent = null)
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
                        //SubPackages.Add(new Vish(this.Controller));
                        Controller.StopBeeper();
                    }
                    Controller.SetActivePackage(SubPackages.Single(p => p.Name == "Vish"));
                    DispatchIntent(null, Menu);
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
                //var bots = new Bots(this.Controller);
                //Controller.StopBeeper();
                //if (bots.Initialized)
               // {
                    //SubPackages.Add(new Bots(this.Controller));
               // }
                //else
                //{
                //    SayErrorLine("The Bots package failed to initialize.");
                    return;
                //}
                
            }
            Controller.ActivePackage = SubPackages.Single(p => p.Name == "Bots"); 

            if (CurrentContext.StartsWith("MENU_"))
            {
                DispatchIntent(null, Menu);
            }
            else
            {
                DispatchIntent(null, Menu);
            }

        }
        #endregion
    }
}
