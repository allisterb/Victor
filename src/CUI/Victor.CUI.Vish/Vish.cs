using System;
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

        public override string[] MenuNames { get; } = { "VISH_PACKAGES" };

        public override string[] ItemNames { get; } = Array.Empty<string>();

        #region Intent
        public override void Welcome(Intent intent = null)
        {
            SayInfoLine("Welcome to the Voice Interactive Shell.");
            SayInfoLine("Say {0} to see a menu of options or {1} to get help. Enter {2} if you want to quit.\nTo get background information on any part of Vish enter {3}.",
                "menu", "help", "exit", "info");
        }

        public override void Menu(Intent intent)
        {
            Controller.SetContext("MENU_VISH_PACKAGES", intent, Menu);
            SayInfoLine("1 {0}", "Red Hat OpenShift");
        }

        public override void Help(Intent intent = null)
        {
            throw new NotImplementedException();
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
                    DispatchIntent(null, Controller.ActivePackage.Menu);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        #endregion
    }
}

