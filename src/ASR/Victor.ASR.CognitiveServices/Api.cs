using System;

using Victor;

namespace Victor.ASR.CognitiveServices
{
    public class AzureRecognizer : Api
    {

        #region Properties
        
        //public FormRecognizerClient Client;
        //public DocumentAnalysisClient Client3;
        private static readonly string Endpoint = "https://victor-du.cognitiveservices.azure.com/";
        private static readonly string ApiKey = Config("AZURE_SPEECHRECOGNIZER_KEY");
        
        #endregion
    }
}
