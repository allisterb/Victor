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
            MenuHandlers["HOME_PACKAGES"] = GetPackagesMenuItem;
            MenuIndexes["HOME_PACKAGES"] = 1;
            Initialized = NLUEngine.Initialized;
            if (!Initialized)
            {
                SayErrorLine("NLU engine for package {0} did not initialize. Exiting.", this.Name);
                Program.Exit(ExitResult.UNKNOWN_ERROR);
            }
        }
        #endregion

        #region Overriden members
        public override string[] VariableNames { get; } = { "HOME_NAME" };

        public override string[] MenuItemNames { get; } = { "HOME_PACKAGES" };

        public override bool ParseIntent(CUIContext context, DateTime time, string input)
        {
            var intent = NLUEngine.GetIntent(input);
            if (Controller.DebugEnabled)
            {
                DebugIntent(intent);
            }

            if (intent.Top.Score < 0.6)
            {
                return false;
            }
            else
            {
                switch (intent.Top.Label)
                {
                    case "exit":
                        Exit(intent);
                        break;
                    case "help":
                        Help(intent);
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
                    case "menu":
                        DispatchIntent(intent, Controller.ActivePackage.Menu);
                        break;
                    default:
                        break;
                }
                return true;
            }
        }

        public override void Welcome(Intent intent = null)
        {
            Controller.SetContext("WELCOME");
            SayInfoLine("Welcome to Victor CX."); 
            SayInfoLine("Enter {0} to see a menu of options or {1} to get help. Enter {2} if you want to quit.", "menu", "help", "exit");
        }
        public override void Menu(Intent intent)
        {
            Controller.SetContext("MENU_PACKAGES");
            SayInfoLine("Select a package to use.");
            SayInfoLine("1 {0}", "Vish");
        }

        #endregion

        #region Methods

        #region CUI Functions

        public void Exit(Intent intent)
        {
            SayInfoLine("Shutting down...");
            Program.Exit(ExitResult.SUCCESS);
        }

        protected void Hello(Intent intent)
        {
            var name = intent.Entities.Length > 0 ? intent.Entities.First().Value : "";
            if (!string.IsNullOrEmpty(name))
            {
                SayInfoLine("Hello {0} welcome to the Victor CX auditory user interface.", name);
                Variables["Name"] = name;
            }
            else 
            {
                SayInfoLine("Hello, welcome to the Victor CX auditory user interface.");
            }
            SayInfoLine("Enter {0} to see a menu of options or {1} to get help. Enter {2} if you want to quit.", "menu", "help", "exit");
        }

        protected void Help(Intent intent)
        {
            var feature = intent.Entities.Length > 0 ? intent.Entities.FirstOrDefault(e => e.SlotName == "feature")?.Value : null;
            var package = intent.Entities.Length > 0 ? intent.Entities.FirstOrDefault(e => e.SlotName == "package")?.Value : null;
            var function = intent.Entities.Length > 0 ? intent.Entities.FirstOrDefault(e => e.SlotName == "function")?.Value : null;
            if (!string.IsNullOrEmpty(feature))
            {
                feature = new string(feature.Where(c => Char.IsLetterOrDigit(c)).ToArray());
            }
            var context = Context.Count > 0 ? Context.Peek().Label : "";
            switch (feature)
            {
                case null:
                case "":
                case "this":
                    switch (context)
                    {
                        case "WELCOME":
                            SayInfoLine("Victor CX is an auditory conversational user interface for interacting with an organisation\'s online services like product registration and on-boarding, product documentation, customer service and support.");
                            break;

                        default:
                            break;
                    }
                    break;
                case "nlu":
                    SayInfoLine("Victor CX uses natural language understanding to understand a user's intent and the entities that are part of that intent.");
                    SayInfoLine("A user does not have to enter an exact phrase or command but can express their intent using natural language and different phrases and synonymns.");
                    SayInfoLine("You can enable debug mode by entering {0} or {1}. For each user input this will print information about the intent and entities extracted by the NLU engine.", "enable debug", "debug on");
                    break;
                default:
                    SayInfoLine("No help so far for {0}.", feature);
                    break;
            }

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
                    default:
                        SayErrorLine("Sorry I don't know how to enable that.");
                        break;
                }
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
                    Controller.ActivePackage = SubPackages.Single(p => p.Name == "Vish");
                    DispatchIntent(null, Controller.ActivePackage.Menu);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        #endregion
    }
}
