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
        public OpenShift(CUIController controller, NLUEngine engine, CancellationToken ct) : base("OpenShift", engine, controller, ct)
        {
            ApiUrl = Config("CUI_VISH_OPENSHIFT_URL");
            ApiToken = Config("CUI_VISH_OPENSHIFT_TOKEN");
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
        #endregion

        #region Properties
        protected string ApiToken { get; }

        protected string ApiUrl { get; }
        
        protected OpenShiftAPIwithKubernetes Client { get; }
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

        public async Task<Iok8sapicorev1PodList> GetPods()
        {
            ThrowIfNotInitialized();
            return await Client.ListCoreV1NamespacedPodAsync("evals25-shared-7daa");
            
            //var r = RestClient.Execute(new RestRequest("/"));
            //if (r.StatusCode == HttpStatusCode.OK)
            //{
            //    return RESTResources.FromJson(r.Content);//JsonConvert.DeserializeObject<Resource[]>(r.Content);

            //}
            //else
            //{
            //    throw new Exception();
            //}

        }

        
        #endregion
    }
}
