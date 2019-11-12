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
            MenuHandlers.Add("PACKAGES", GetPackagesMenuItem);
            MenuIndexes.Add("PACKAGES", 1);
            Initialized = NLUEngine.Initialized;
            if (!Initialized)
            {
                SayErrorLine("NLU engine for package {0} did not initialize. Exiting.", this.Name);
                Program.Exit(ExitResult.UNKNOWN_ERROR);
            }
        }
        #endregion

        #region Overriden methods
        public override void Menu(Intent intent)
        {
            Controller.SetContext("MENU_PACKAGES");
            SayInfoLine("Select a package to use.");
            SayInfoLine("1. Vish");
        }

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
                        DispatchIntent(intent, Exit);
                        break;
                    case "help":
                        DispatchIntent(intent, Help);
                        break;
                    case "hello":
                        DispatchIntent(intent, Hello);
                        break;
                    case "enable":
                        Enable(intent);
                        break;
                    case "disable":
                        Disable(intent);
                        break;
                    case "menu":
                        DispatchIntent(intent, Menu);
                        break;
                    default:
                        break;
                }
                return true;
            }
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
            SayInfoLine("Hello {0} welcome to the Victor CX auditory user interface.", name);
            SayInfoLine("Enter help to see the help pagaes, packages to see available packages.");
        }

        protected void Help(Intent intent)
        {
            var feature = intent.Entities.Length > 0 ? intent.Entities.FirstOrDefault(e => e.Kind == "feature")?.Value : null;
            var package = intent.Entities.Length > 0 ? intent.Entities.FirstOrDefault(e => e.Kind == "package")?.Value : null;
            var function = intent.Entities.Length > 0 ? intent.Entities.FirstOrDefault(e => e.Kind == "function")?.Value : null;
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
                    SayInfoLine("Victor CX uses natural language understading to understand a user's intent and the entities that are part of that intent.");
                    SayInfoLine("You can enable NLU debug mode by entering {0}.", "enable debug");
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
                        SubPackages.Add(new Vish(this.Controller, new SnipsNLUEngine(Path.Combine("Engines", "vish"))));
                        Controller.StopBeeper();
                    }
                    
                    DispatchIntent(null, SubPackages.Single(p => p.Name == "Vish").Menu);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        #endregion
    }
}
