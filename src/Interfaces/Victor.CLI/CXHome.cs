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
            MenuHandlers.Add("PACKAGES", GetMenuPackageItem);
            MenuIndexes.Add("PACKAGES", 1);
            Initialized = NLUEngine.Initialized;
            if (!Initialized)
            {
                SayErrorLine("Package not initialized. Exiting.");
                Program.Exit(ExitResult.UNKNOWN_ERROR);
            }
        }
        #endregion

        #region Overriden methods
        public override void Menu(Intent intent)
        {
            SayInfoLine("1. Vish");
        }

        public override bool ParseIntent(CUIContext context, DateTime time, string input)
        {
            var intent = NLUEngine.GetIntent(input);
            if (Controller.DebugEnabled)
            {
                DebugIntent(intent);
                if (intent.Top.Label == "enable")
                {
                    Enable(intent);
                }
                else if (intent.Top.Label == "disable")
                {
                    Disable(intent);
                }
                else if (intent.Top.Label == "menu")
                {
                    DispatchIntent(intent, Menu);
                }
                else if (intent.Top.Label == "exit")
                {
                    DispatchIntent(intent, Exit);
                }
                return true;
            }
            else if (intent.Top.Score < 0.8)
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
                    default:
                        break;
                }
                return true;
            }
        }
        #endregion

        #region Functions

        #region General
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
            var feature = intent.Entities.Length > 0 ? intent.Entities.First().Value : null;
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
                            SayInfoLine("Debug enabled. NLU information will be output and commands won't be executed.");
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
        #endregion

        protected void GetMenuPackageItem(int i)
        {
            switch(i - 1)
            {
                case 0:
                    if (!SubPackages.Any(p => p.Name == "Vish"))
                    {
                        SubPackages.Add(new Vish(this.Controller, new SnipsNLUEngine(Path.Combine("Engines", "vish"))));
                    }
                    SubPackages.Single(p => p.Name == "Vish").Menu(null);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
            SubPackages[i - 1].DispatchIntent(null, SubPackages[i - 1].Menu);
        }
    }
}
