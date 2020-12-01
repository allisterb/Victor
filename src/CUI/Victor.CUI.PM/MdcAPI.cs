using System;
using System.Threading.Tasks;

using GraphQL.Client;

using Victor.CUI.PM.Models;

namespace Victor.CUI.PM
{
    public class MdcApi : Api
    {
        public static async Task<BoardsQueryResultData> GetBoards()
        {
            var graphQLClient = new GraphQLClient("https://api.monday.com/v2");
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
            try
            {
                var graphQLResponse = await graphQLClient.PostAsync(boardsQueryRequest);
                return graphQLResponse.Data.ToObject<BoardsQueryResultData>();
            }
            catch(Exception e)
            {
                Error(e, "Error retrieving boards from monday.com API.");
                return null;
            }
            
        }
    }
}
