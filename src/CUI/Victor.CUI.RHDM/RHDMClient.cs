using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

using RestSharp;

namespace Victor
{
    public class RHDMClient : Api
    {
        #region Constructors
        public RHDMClient(string serverUrl, CancellationToken ct) : base(ct)
        {
            ServerUrl = new Uri(serverUrl);
            RestClient = new RestClient(ServerUrl);
            Initialized = true;
        }

        public RHDMClient(string url) : this(url, Ct) { }
        #endregion

        #region Properties
        public Uri ServerUrl { get; }
        protected RestClient RestClient { get; }
        #endregion

        #region Methods
        /*
        public string Get()
        {
            RestClient.
        }
        */
        #endregion
    }
}
