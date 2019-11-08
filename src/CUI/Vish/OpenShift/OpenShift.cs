using System;
using System.IO;
using System.Threading;


using RestSharp; 

namespace Victor
{
    public class OpenShift : CUIPackage
    {
        #region Constructors
        public OpenShift(CUIController controller, NLUEngine engine, CancellationToken ct) : base("OpenShift", engine, controller, ct)
        {
            ApiUrl = Config("CUI_VISH_OPENSHIFT_URL");
            ApiToken = Config("CUI_VISH_OPENSHIFT_TOKEN");
            if (!string.IsNullOrEmpty(ApiToken) && !string.IsNullOrEmpty(ApiUrl))
            {
                RestClient = new RestClient(new Uri(ApiUrl));
                RestClient.AddDefaultHeader("Authorization", "Bearer " + ApiToken);
                Initialized = true;
            }
            else if (string.IsNullOrEmpty(ApiUrl))
            {
                SayErrorLine("I could not determine your OpenShift API URL.");
            }
            else if (string.IsNullOrEmpty(ApiToken))
            {
                SayErrorLine("I could not determine your OpenShift service API token.");
            }
        }
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
                    case "menu":
                        DispatchIntent(intent, Menu);
                        return true;
                    default:
                        return false;
                }
            }
        }
        #endregion

        #region Methods
        public override void Menu(Intent intent)
        {
            Controller.SetContext("MENU_OPENSHIFT");
            SayInfoLine("RedHat OpenShift");
            SayInfoLine("1. Get pods");
        }

        protected void GetResources()
        {
            ThrowIfNotInitialized();
            var r = RestClient.Execute(new RestRequest("/builds"));
            File.WriteAllText("builds.json", r.Content);
        }
        #endregion
    }
}
