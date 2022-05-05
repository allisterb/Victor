using System;
using System.Threading;
using Victor.CUI;
namespace Victor.Vision
{
    #region Constructors
    public class AzureFormRecognizer : Api
    {
        #region Constructors
        public AzureFormRecognizer(Controller controller, CancellationToken ct): base(ct)
        {

        }
        #endregion

        #region Properties
        public static readonly string ApiKey = Config("AZURE_FORMRECOGNIZER_KEY");
        #endregion
    }
    #endregion


}
