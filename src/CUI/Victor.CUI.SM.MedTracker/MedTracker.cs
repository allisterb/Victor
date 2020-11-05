using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Victor
{
    public class MedsTracker : CUIPackage
    {
        #region Constructors
        public MedTracker(CUIController controller, CancellationToken ct) : base("Vish", new SnipsNLUEngine(Path.Combine("Engines", "vish")), controller, ct)
        {
            Intents.Add("launch", Launch);
            MenuIndexes["VISH_PACKAGES"] = 3;
            MenuHandlers["VISH_PACKAGES"] = GetPackagesMenuItem;
            Initialized = NLUEngine.Initialized;
        }

        public MedsTracker(CUIController controller) : this(controller, Ct) { }

        #endregion

        #region Overriden members
        public override string[] VariableNames { get; } = { };

        public override string[] MenuNames { get; } = { "PACKAGES" };

        public override string[] ItemNames { get; } = Array.Empty<string>();

        #region Intents
        public override void Menu(Intent intent)
        {
            ThrowIfNotInitialized();
            SetMenuContext("PACKAGES", intent, Menu);
            SayInfoLine("1 {0}", "Red Hat OpenShift");
        }

        public override void Help(Intent intent = null)
        {
            ThrowIfNotInitialized();
            var context = CurrentContext;
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

        #region Intents
        public void Launch(Intent intent)
        {
            var input = intent.Input.ToLower();
            if (ObjectEmpty(intent))
            {
                if (input.Contains("openshift") || input.Contains("open shift"))
                {
                    LoadOpenShiftPackage();
                }
                else
                {
                    SayErrorLine("Sorry I dont know what you mean. Say something like {0}.", "launch openshift");
                }
            }
            else
            {
                var (feature, package, function) = GetIntentFeaturePackageFunction(intent);
                if (string.IsNullOrEmpty(package))
                {
                    SayErrorLine("No package to launch.");
                    return;
                }
                else
                {
                    if (package == "openshift")
                    {
                        LoadOpenShiftPackage();
                    }
                    else
                    {
                        SayErrorLine("I don't know how to launch package {0}.", package);
                    }
                }
            }
        }
        #endregion

        public void LoadOpenShiftPackage()
        {
            if (!SubPackages.Any(p => p.Name == "OpenShift"))
            {
                SayInfoLine("Loading OpenShift package...");
                Controller.StartBeeper();
                var _oc = new OpenShift(this.Controller);
                Controller.StopBeeper();
                if (_oc.Initialized)
                {
                    SubPackages.Add(_oc);
                }
                else
                {
                    SayErrorLine("The OpenShift package failed to initialize.");
                    return;
                }
            }
            var oc = SubPackages.Single(p => p.Name == "OpenShift");
            Controller.ActivePackage = oc;

            if (CurrentContext.StartsWith("MENU_"))
            {
                DispatchIntent(null, Controller.ActivePackage.Menu);
            }
            else
            {
                DispatchIntent(null, Controller.ActivePackage.Welcome);
            }

        }
        protected void GetPackagesMenuItem(int i)
        {
            switch (i - 1)
            {
                case 0:
                    LoadOpenShiftPackage();
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        #endregion
    }
}

