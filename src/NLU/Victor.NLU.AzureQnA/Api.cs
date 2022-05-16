using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker;
using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker.Models;

using Victor.CUI;

namespace Victor.NLU
{
    public class AzureQnA : Api
    {

        #region Constructors
        public AzureQnA(Controller controller, CancellationToken ct) : base(ct)
        {
            if (string.IsNullOrEmpty(ApiKey))
            {
                Error("AZURE_QNA_KEY environment variable not set.");
                Initialized = false;
                return;
            }
            else
            {
                Client = GetClient();
                RuntimeClient = GetRuntimeClient();
                Initialized = true;
            }
        }
        #endregion

        #region Properties
        private static readonly string Endpoint = "https://victor-ta.cognitiveservices.azure.com/";
        private static readonly string ApiKey = Config("AZURE_QNA_KEY");
        public QnAMakerClient Client;
        public QnAMakerRuntimeClient RuntimeClient;
        #endregion

        #region Methods
        private static QnAMakerClient GetClient()
        {
            var credential = new ApiKeyServiceClientCredentials(ApiKey);
            var client = new QnAMakerClient(credential)
            {
                Endpoint = Endpoint
            };
            return client;
        }

        private static QnAMakerRuntimeClient GetRuntimeClient()
        {
            var credential = new EndpointKeyServiceClientCredentials(ApiKey);
            var client = new QnAMakerRuntimeClient(credential);
            return client;
        }

        public List<string> GetKnowledgebases()
        {
            var dto = Client.Knowledgebase.ListAllAsync(this.CancellationToken).Result;
            return dto.Knowledgebases.Select(r => r.Id).ToList();
        }
        #endregion
    }
}
