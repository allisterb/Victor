using System;
using System.IO;
using System.Linq;
using System.Threading;


using RestSharp;

namespace Victor
{
    public class Vish : CUIPackage
    {
        #region Constructors
        public Vish(CUIController controller, NLUEngine engine, CancellationToken ct) : base("Vish", engine, controller, ct)
        {
            SubPackages.Add(new OpenShift(controller, engine, ct));
            MenuIndexes.Add("VISH_OPENSHIFT", 1);
            MenuHandlers.Add("VISH_OPENSHIFT", GetMenuPackageItem);
            Initialized = NLUEngine.Initialized && SubPackages.All(p => p.Initialized);
        }

        public Vish(CUIController controller, NLUEngine engine) : this(controller, engine, Ct) {}

        #endregion

            #region Properties
        protected string ApiToken { get; }

        protected string ApiUrl { get; }

        protected RestClient RestClient { get; }
        #endregion


        #region Overriden methods
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
        #endregion

        #region Methods
        public override void Menu(Intent intent)
        {
            SayInfoLine("1. ", "RedHat OpenShift");
            
            Controller.SetContext("MENU_VISH_OPENSHIFT");
        }

        protected void GetMenuPackageItem(int i) => SubPackages[i - 1].DispatchIntent(null, SubPackages[i - 1].Menu);

        protected void GetResources()
        {
            ThrowIfNotInitialized();
            var r = RestClient.Execute(new RestRequest("/builds"));
            File.WriteAllText("builds.json", r.Content);
        }
        #endregion
    }
}
