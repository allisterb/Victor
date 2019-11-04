using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Victor
{
    public class EDDI : Api
    {
        public EDDI(Uri baseUrl, CancellationToken ct) : base(ct)
        {
            Client = new EDDIClient(baseUrl.ToString(), HttpClient);
            Initialized = true;
        }

        public EDDI(Uri baseUrl) : this(baseUrl, Api.Ct) { }

        public Dictionary<string, string> GetBots()
        { }

        public EDDIClient Client { get; }
    }
}
