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
            return dto.Knowledgebases.Select(r => !string.IsNullOrEmpty(r.Name) ? r.Name : r.Id).ToList();
        }

        public string GetAnswer(string kb, string q)
        {
            var queryDTO = new QueryDTO();
            queryDTO.Question = q;
            queryDTO.Top = 3;
            queryDTO.IsTest = true;
            queryDTO.AnswerSpanRequest = new QueryDTOAnswerSpanRequest()
            {
                Enable = true,
                ScoreThreshold = 5.0,
                TopAnswersWithSpan = 1
            };
            var dto = Client.Knowledgebase.GenerateAnswerAsync(kb, queryDTO).Result;
            return dto.Answers.First().Answer;
        }
        #endregion
    }
}
