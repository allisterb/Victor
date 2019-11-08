using System;
using System.IO;
using System.Threading;


using RestSharp; 

namespace Victor
{
    public class OpenShift : CUIPackage
    {
        #region Constructors
        public OpenShift(CUIController controller, CancellationToken ct) : base("OpenShift", controller, ct)
        {
            ApiUrl = Config("CUI_VISH_OPENSHIFT_URL");
            ApiToken = Config("CUI_VISH_OPENSHIFT_TOKEN");
            if (!string.IsNullOrEmpty(ApiToken) && !string.IsNullOrEmpty(ApiUrl))
            {
                RestClient = new RestClient(new Uri(ApiUrl));
                RestClient.AddDefaultHeader("Authorization", "Bearer " + ApiToken);
                Initialized = true;
            }
        }
        #endregion

        #region Properties
        protected string ApiToken { get; }

        protected string ApiUrl { get; }
        
        protected RestClient RestClient { get; }
        #endregion


        #region Methods
        protected void GetResources()
        {
            ThrowIfNotInitialized();
            var r = RestClient.Execute(new RestRequest("/builds"));
            File.WriteAllText("builds.json", r.Content);
        }
        #endregion
    }
}
