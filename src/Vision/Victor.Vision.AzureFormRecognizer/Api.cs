using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;

using Victor.CUI;

namespace Victor.Vision
{
    #region Constructors
    public class AzureFormRecognizer : Api
    {
        #region Constructors
        public AzureFormRecognizer(Controller controller, CancellationToken ct): base(ct)
        {
            Client = AuthenticateClient();
            Initialized = true;
        }
        #endregion

        #region Properties
        public FormRecognizerClient Client;
        private static readonly string Endpoint = "https://victor-du.cognitiveservices.azure.com/";
        private static readonly string ApiKey = Config("AZURE_FORMRECOGNIZER_KEY");
        private static readonly AzureKeyCredential credential = new AzureKeyCredential(ApiKey);
        #endregion

        #region Methods
        private static FormRecognizerClient AuthenticateClient()
        {
            var credential = new AzureKeyCredential(ApiKey);
            var client = new FormRecognizerClient(new Uri(Endpoint), credential);
            return client;
        }

        private async Task<FormPageCollection> RecognizeDocument(string filename)
        {
            var fs = File.OpenRead(filename);
            //var invoiceUri = "https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/simple-invoice.png";
            return await Client
                .StartRecognizeContent(fs)
                .WaitForCompletionAsync();
           
            /*
            foreach (FormPage page in formPages)
            {
                Debug($"Form Page {page.PageNumber} has {page.Lines.Count} lines.");

                for (int i = 0; i < page.Lines.Count; i++)
                {
                    FormLine line = page.Lines[i];
                    Debug($"    Line {i} has {line.Words.Count} word{(line.Words.Count > 1 ? "s" : "")}, and text: '{line.Text}'.");
                }

                for (int i = 0; i < page.Tables.Count; i++)
                {
                    FormTable table = page.Tables[i];
                    Debug($"Table {i} has {table.RowCount} rows and {table.ColumnCount} columns.");
                    foreach (FormTableCell cell in table.Cells)
                    {
                        Debug($"    Cell ({cell.RowIndex}, {cell.ColumnIndex}) contains text: '{cell.Text}'.");
                    }
                }
            }
            */
        }
        #endregion
    }
    #endregion


}
