﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Victor.CUI.DU.Models;
using Victor.Vision;

namespace Victor.CUI.DU
{
    public class DUHome : Package
    {
        #region Constructors
        public DUHome(Controller controller) : base("Documents", new SnipsNLUEngine(Path.Combine("Engines", "fn")), controller)
        {
            Features = Menus["FEATURES"] = new Menu("FEATURES", GetFeaturesMenuItem, "Open", "Scan");
            DocItems = Items["Docs"] = new Items("Docs", typeof(Doc), ListDocs, DescribeDoc);
        
            Recognizer = new AzureFormRecognizer(this.Controller, this.CancellationToken);
            Initialized = NLUEngine.Initialized && Recognizer.Initialized;
            if (!Initialized)
            {
                SayErrorLine("NLU engine for package {0} did not initialize. Exiting.", this.Name);
                Controller.Exit(ExitResult.UNKNOWN_ERROR);
            }
        }
        #endregion

        #region Properties
        public Menu Features { get; }

        public Items DocItems { get; }

        public AzureFormRecognizer Recognizer {get; }
        
        public List<Doc> Docs { get; protected set; }
        #endregion
        
        #region Overriden members

        #region Intents
        public override void Welcome(Intent intent = null)
        {
            base.Welcome(intent);
            SayInfoLine("Welcome to Victor DU.");
            SayInfoLine("Say {0} to show the main menu or {1} to get more background information. Say {2} to exit.", "menu", "info", "exit");
        }

        protected override void Info(Intent intent = null)
        {
            if (EmptyEntities(intent))
            {
                SayInfoLine("Victor FN is a personal finance and accounting program designed for vision-impaired and differently-abled users.");
                SayInfoLine("This is the Victor auditory conversational user interface for managing accounts, bills, money transfers");            
            }
            else
            {
                var (feature, package, function) = GetIntentFeaturePackageFunction(intent);
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

        protected override void Help(Intent intent)
        {
            var context = CurrentContext;
            if (EmptyEntities(intent))
            {
                switch (context)
                {
                    case "WELCOME_PROJECTMANAGEMENT":

                        SayInfoLine("Victor Poject management program designed for people with vision or other disabilities.");

                        SayInfoLine("Victor SM tasks and features are divided into packages. This is the {0} package which lets you jump to other packages or set global options and variables.", "HOME");
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
                else if (!string.IsNullOrEmpty(package))
                {
                    switch (package)
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

        protected override void Menu(Intent intent)
        {
            switch(CurrentContext)
            {
                case "WELCOME_DOCUMENTS":
                    SetMenuContext("FEATURES");
                    SayInfoLine("Select a feature to use.");
                    SayInfoLine("1. {0}", "Open");
                    SayInfoLine("2. {0}", "Scan");
                    SayInfoLine("3. {0}", "Monitor");
                    break;
                default:
                    SayErrorLine("Unknown controller context: {0}.", CurrentContext);
                    break;
            }
        }

        #endregion

        public override string[] VariableNames { get; } = { "FILE_NAME" };

        public override string[] MenuNames { get; } = { "FEATURES" };

        public override string[] ItemNames { get; } = Array.Empty<string>();


        #endregion

        #region Methods

        #region Intents
        public void Open(Intent intent)
        {
            Context.Pop();
            var filename = GetVar("FILE_NAME");
            if (filename.ToUpper() == "CANCEL")
            {
                Context.Pop();
                DispatchIntent(null, Menu);
            }
            else
            {
                SayInfoLineIfDebug($" Got {filename}");
                if (!File.Exists(filename))
                {
                    SayErrorLine($"Sorry, I couldn't find the file {filename}. Try entering it again");
                    GetVariableInput("FILE_NAME", Open);
                }
            }
            
        }
        #endregion

        #region Items
        protected void GetFeaturesMenuItem(int i)
        {
            switch(i - 1)
            {
                case 0:
                    GetVariableInput("FILE_NAME", Open);
                    break;

                default:
                    throw new IndexOutOfRangeException();
            }
        }

        protected void ListDocs(Intent intent)
        {
            ThrowIfNotInitialized();

        }

        protected void DescribeDoc(int index)
        {
            var a = DocItems.Get<Doc>(index);
            //SayInfoLine("Name: {0}.", a.Name);
        }
            
        #endregion

        #region Azure API
        
        #endregion

        #endregion
    }
}
