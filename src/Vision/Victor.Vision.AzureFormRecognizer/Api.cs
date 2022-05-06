using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;
using Azure.AI.FormRecognizer.DocumentAnalysis;

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
            Client3 = AuthenticateClient3();
            Initialized = true;
        }
        #endregion

        #region Properties
        public FormRecognizerClient Client;
        public DocumentAnalysisClient Client3;
        private static readonly string Endpoint = "https://victor-du.cognitiveservices.azure.com/";
        private static readonly string ApiKey = Config("AZURE_FORMRECOGNIZER_KEY");
        #endregion

        #region Methods
        private static FormRecognizerClient AuthenticateClient()
        {
            var credential = new AzureKeyCredential(ApiKey);
            var client = new FormRecognizerClient(new Uri(Endpoint), credential);
            return client;
        }

        private static DocumentAnalysisClient AuthenticateClient3()
        {
            var credential = new AzureKeyCredential(ApiKey);
            var client = new DocumentAnalysisClient(new Uri(Endpoint), credential);
            return client;
        }

        public AnalyzeResult AnalyzeDocument(string modelid, string filename)
        {
            var fs = File.OpenRead(filename);
            var op = Client3.StartAnalyzeDocument(modelid, fs, cancellationToken: this.CancellationToken);
            return op.WaitForCompletion(Ct).Value;
           
            //return await Client
            //    .StartRecognizeContent(fs)
            //    .WaitForCompletionAsync();
           
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
