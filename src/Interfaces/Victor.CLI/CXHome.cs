using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Victor.CLI
{
    public class CXHome : CUIPackage
    {
        public CXHome(CUIController controller) : base("Home", new SnipsNLUEngine(Path.Combine("Engines", "home")), controller)
        {
            MenuHandlers.Add("MENU_PACKAGE", GetMenuPackageItem);
            Initialized = NLUEngine.Initialized;
        }

        public override bool ParseIntent(CUIContext context, DateTime time, string input)
        {
            var intent = NLUEngine.GetIntent(input);
            if (Controller.DebugEnabled)
            {
                SayInfoLine("Context: {0}, Package: {1}, Intent: {2} Score: {3}.", Context.PeekIfNotEmpty().Label, this.Name, intent.Top.Label, intent.Top.Score);
                foreach (var e in intent.Entities)
                {
                    SayInfoLine("Entity:{0} Value:{1}.", e.Entity, e.Value);
                }
                if (intent.Top.Label == "enable")
                {
                    Enable(intent);
                }
                else if (intent.Top.Label == "disable")
                {
                    Disable(intent);
                }
                else if (intent.Top.Label == "exit")
                {
                    Exit();
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
                        Exit();
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
                    default:
                        break;
                }
                return true;
            }
        }

        #region Features

        #region General
        public void Exit()
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

        protected void GetMenuPackageItem(int i) => throw new NotImplementedException();
        #endregion
        #endregion
    }
}
