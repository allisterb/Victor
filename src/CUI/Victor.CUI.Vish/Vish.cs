﻿using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Victor
{
    public class Vish : CUIPackage
    {
        #region Constructors
        public Vish(CUIController controller, CancellationToken ct) : base("Vish", new SnipsNLUEngine(Path.Combine("Engines", "vish")), controller, ct)
        {
            MenuIndexes["VISH_PACKAGES"] = 3;
            MenuHandlers["VISH_PACKAGES"] = GetPackagesMenuItem;
            Initialized = NLUEngine.Initialized;
        }

        public Vish(CUIController controller) : this(controller, Ct) {}

        #endregion

        #region Overriden members
        public override string[] VariableNames { get; } = { };

        public override string[] MenuNames { get; } = { "PACKAGES" };

        public override string[] ItemNames { get; } = Array.Empty<string>();

        #region Intents
        public override void Menu(Intent intent)
        {
            Controller.SetContext("MENU_VISH_PACKAGES", intent, Menu);
            SayInfoLine("1 {0}", "Red Hat OpenShift");
        }

        public override void Help(Intent intent = null)
        {
            var context = Context.Count > 0 ? Context.Peek().Label : "";

            if (intent == null || intent.Entities.Length == 0)
            {
                switch (context)
                {
                    case "WELCOME_VISH":
                        SayInfoLine("Welcome to the Voice Interactive Shell.");
                        SayInfoLine("Say {0} to see a menu of Vish packages available or {1} or {2} to go back to HOME.",
                            "menu", "back", "home");
                        break;
                    case "MENU_VISH_PACKAGES":
                        SayInfoLine("Enter the number associated with the Vish package you want to select.");
                        break;
                    default:
                        SayErrorLine("Unknown VISH context: {0}.", context);
                        SayInfoLine("Say {0} to enable debug mode.", "enable debug");
                        break;
                }
            }
            else
            {
                var package = intent.Entities.Length > 0 ? intent.Entities.FirstOrDefault(e => e.SlotName == "package")?.Value : null;
             
                if (string.IsNullOrEmpty(package))
                {
                    SayInfoLine("Sorry I didn't understand what you said.");
                    SayInfoLine("Say {0} to see a menu of Vish packages available or {1} or {2} to go back to HOME.", "menu", "back", "home");

                }
                else
                {
                    package = new string(package.Where(c => Char.IsLetterOrDigit(c)).ToArray());
                    switch (package)
                    {
                        case "this":
                            Help(null);
                            break;
                        case "openshift":
                            SayInfoLine("The {0} package helps you administer a Red Hat OpenShift cluster.", "RedHat OpenShift");
                            break;
                        case "menu":
                            SayInfoLine("Enter the number associated with the Vish package you want to select.");
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
            throw new NotImplementedException();
        }
        #endregion

        #endregion

        #region Properties
        protected string ApiToken { get; }

        protected string ApiUrl { get; }
        #endregion

        #region Methods
        protected void GetPackagesMenuItem(int i)
        {
            switch (i - 1)
            {
                case 0:
                    if (!SubPackages.Any(p => p.Name == "OpenShift"))
                    {
                        SayInfoLine("Loading OpenShift package...");
                        Controller.StartBeeper();
                        var _oc = new OpenShift(this.Controller);
                        if (_oc.Initialized)
                        {
                            SubPackages.Add(_oc);
                        }
                        Controller.StopBeeper();
                    }
                    var oc = SubPackages.Single(p => p.Name == "OpenShift");
                    if (oc.Initialized)
                    {
                        Controller.ActivePackage = oc;
                    }
                    else
                    {
                        SayErrorLine("The OpenShift package failed to initialize.");
                    }
                    DispatchIntent(null, Controller.ActivePackage.Welcome);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        #endregion
    }
}

