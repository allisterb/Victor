using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Victor
{
    public partial class EDDIClient
    {
        //partial void UpdateJsonSerializerSettings(Newtonsoft.Json.JsonSerializerSettings settings);
        //partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url);
        //partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, System.Text.StringBuilder urlBuilder);
        partial void ProcessResponse(System.Net.Http.HttpClient client, System.Net.Http.HttpResponseMessage response)
        {
            LastStatusCode = (int)response.StatusCode;
            LastResponse = response;
          
        }

        public static int LastStatusCode { get; protected set; }

        public static HttpResponseMessage LastResponse { get; protected set; }

        public static IEnumerable<string> GetLastResponseHeader(string header)
        {
            if (LastResponse != null && LastResponse.Headers != null && LastResponse.Headers.Contains(header))
            {
                return LastResponse.Headers.GetValues(header);
            }
            else return null;
        }
    }
}
