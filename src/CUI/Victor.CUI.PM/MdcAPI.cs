using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using GraphQL.Client;


using Victor.CUI.PM.Models;

namespace Victor.CUI.PM
{
    public class MdcApi : Api
    {
        public static async Task<BoardsQueryResult> GetBoards()
        {
            var graphQLClient = new GraphQLClient("https://api.monday.com/v2"); // new NewtonsoftJsonSerializer());
            graphQLClient.DefaultRequestHeaders.Add("Authorization", Config("MDC_API_TOKEN"));
            var boardsQueryRequest = new GraphQL.Common.Request.GraphQLRequest
            {
                Query = @"{
                  boards {
                    id
                    name
                    columns {
                      title
                      id
                      type
                    }
                    groups {
    	                title
                      id
                    }
    
                    
                  }
                }"
            };
            
            var graphQLResponse = await graphQLClient.PostAsync(boardsQueryRequest);
            var x = graphQLResponse.Data.ToObject<BoardsQueryResultData>();
            return null;
        }
    }
}
