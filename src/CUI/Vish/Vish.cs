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
            MenuIndexes["VISH_PACKAGES"] = 1;
            MenuHandlers["VISH_PACKAGES"] = GetPackagesMenuItem;
            Initialized = NLUEngine.Initialized;
        }

        public Vish(CUIController controller) : this(controller, Ct) {}

        #endregion

        #region Properties
        protected string ApiToken { get; }

        protected string ApiUrl { get; }
        #endregion

        #region Overriden members
        public override string[] VariableNames { get; } = { };

        public override string[] MenuItemNames { get; } = { "VISH_PACKAGES" };

        public override bool ParseIntent(CUIContext context, DateTime time, string input)
        {
            var intent = NLUEngine.GetIntent(input);
            if (intent.Top.Score < 0.8)
            {
                return false;
            }
            else
            {
                switch (intent.Top.Label)
                {
                    default:
                        return false;
                }
            }
        }

        public override void Welcome(Intent intent = null)
        {
            SayInfoLine("Welcome to the Voice Interactive Shell.");
            SayInfoLine("Enter {0} to see a menu of options or {1} to get help. Enter {2} if you want to quit.", "menu", "help", "exit");
        }
        #endregion

        #region Methods

        #region Functions
        public override void Menu(Intent intent)
        {
            Controller.SetContext("MENU_VISH_PACKAGES", intent, Menu);
            SayInfoLine("1 {0}", "Red Hat OpenShift");   
        }
        #endregion
        protected void GetPackagesMenuItem(int i)
        {
            switch (i - 1)
            {
                case 0:
                    if (!SubPackages.Any(p => p.Name == "OpenShift"))
                    {
                        SayInfoLine("Loading OpenShift package...");
                        Controller.StartBeeper();
                        SubPackages.Add(new OpenShift(this.Controller));
                        Controller.StopBeeper();
                    }
                    Controller.ActivePackage = SubPackages.Single(p => p.Name == "OpenShift");
                    DispatchIntent(null, Controller.ActivePackage.Menu);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        #endregion
    }
}

