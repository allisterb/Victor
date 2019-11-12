using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rest;
using Newtonsoft.Json;

using Victor.CUI.Vish.OpenShift.Client;
using Victor.CUI.Vish.OpenShift.Client.Models;
namespace Victor
{
    public class OpenShift : CUIPackage
    {
        #region Constructors
        public OpenShift(CUIController controller, CancellationToken ct) : base("OpenShift", new SnipsNLUEngine(Path.Combine("Engines", "openshift")),  controller, ct)
        {
            ApiUrl = Config("CUI_VISH_OPENSHIFT_URL");
            ApiToken = Config("CUI_VISH_OPENSHIFT_TOKEN");
            MenuHandlers["OPENSHIFT"] = GetOpenShiftMenuItem;
            MenuIndexes["OPENSHIFT"] = 1;
            if (!string.IsNullOrEmpty(ApiToken) && !string.IsNullOrEmpty(ApiUrl))
            {
                var handler = new HttpClientHandler {};
                Client = new OpenShiftAPIwithKubernetes(new Uri(ApiUrl), new TokenCredentials(ApiToken), handler); 
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

        public OpenShift(CUIController controller) : this(controller, Ct) {}
        #endregion

        #region Properties
        protected string ApiToken { get; }

        protected string ApiUrl { get; }
        
        protected OpenShiftAPIwithKubernetes Client { get; }
        #endregion

        #region Overriden members
        public override string[] VariableNames { get; } = { "PROJECT" };

        public override string[] MenuItemNames { get; } = { "OPENSHIFT" };

        public override string[] OutputNames { get; } = {"PODS" };
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

        public override void Welcome(Intent intent = null)
        {
            SayInfoLine("Welcome to the OpenShift administration package.");
            SayInfoLine("Enter {0} to see a menu of options or {1} to get help. Enter {2} if you want to quit.", "menu", "help", "exit");
        }
        public override void Menu(Intent intent)
        {
            Controller.SetContext("MENU_OPENSHIFT");
            SayInfoLine("RedHat OpenShift");
            SayInfoLine("1 {0}", "Get pods");
        }

        #endregion

        #region Methods
        protected void GetOpenShiftMenuItem(int i)
        {
            switch (i - 1)
            {
                case 0:
                    DispatchIntent(null, GetPods);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        #region Functions
        public void GetPods(Intent intent)
        {
            Controller.SetContext("GETPODS");
            if (string.IsNullOrEmpty(Variables["PROJECT"]))
            {
                SayWarningLine("The {0} variable is not set. Enter the name of the OpenShift project.", "PROJECT");
                GetInput("PROJECT");

            }
            else
            {
                SayInfoLine("Get pods for project {0}.", Variables["PROJECT"]);
                Controller.StartBeeper();
                var pods = GetPods(Variables["PROJECT"], null).Result;
                Controller.StopBeeper();
                SayInfoLine("Got {0} pods for project {1}.", pods.Items.Count, Variables["PROJECT"]);
            }
           
        }
        #endregion

        #region OpenShift API
        public async Task<Iok8sapicorev1PodList> GetPods(string ns, string label)
        {
            ThrowIfNotInitialized();
            return await Client.ListCoreV1NamespacedPodAsync(namespaceParameter: "evals25-shared-7daa", labelSelector: label);
        }
        #endregion

        #endregion
    }
}
