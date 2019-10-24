using System;
using System.Collections.Generic;
using System.Text;

namespace Victor.CUI.EDDI
{
    public partial class Client
    {
        //partial void UpdateJsonSerializerSettings(Newtonsoft.Json.JsonSerializerSettings settings);
        //partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url);
        //partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, System.Text.StringBuilder urlBuilder);
        partial void ProcessResponse(System.Net.Http.HttpClient client, System.Net.Http.HttpResponseMessage response)
        {

        }

    }
}
