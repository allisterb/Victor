using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Acklann.Plaid;
using Acklann.Plaid.Accounts;
namespace Victor.CUI.FN
{
    public class Plaid
    {
        #region Constructors
        public Plaid(Controller controller)
        {
            Controller = controller;
            Client = new PlaidClient(System.Environment.GetEnvironmentVariable("PLAID_SANDBOX_CLIENT_ID"), 
                System.Environment.GetEnvironmentVariable("PLAID_SANDBOX_SECRET"),
                System.Environment.GetEnvironmentVariable("PLAID_SANDBOX_ACCESS_TOKEN"), Acklann.Plaid.Environment.Sandbox);
            
        }
        #endregion

        
        #region Properties
        public  Controller Controller {get;}
        public PlaidClient Client {get; }
        #endregion

        #region Methods
        public async Task<Acklann.Plaid.Entity.Account[]> GetAccounts()
        {
            var a = await Client.FetchAccountAsync(new GetAccountRequest() 
            {
                ClientId = System.Environment.GetEnvironmentVariable("PLAID_SANDBOX_CLIENT_ID"),
                Secret = System.Environment.GetEnvironmentVariable("PLAID_SANDBOX_SECRET"),
                AccessToken = System.Environment.GetEnvironmentVariable("PLAID_SANDBOX_ACCCESS_TOKEN")
            });
            if (a.IsSuccessStatusCode)
            {
                return a.Accounts;
            }
            else
            {
                Controller.SayErrorLine(a.Exception.ErrorMessage);
                return Array.Empty<Acklann.Plaid.Entity.Account>();
            }
        }
        #endregion
    }
}