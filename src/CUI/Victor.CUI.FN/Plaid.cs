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
        public Plaid()
        {
            Client = new PlaidClient(System.Environment.GetEnvironmentVariable("PLAID_SANDBOX_CLIENT_ID"), 
                System.Environment.GetEnvironmentVariable("PLAID_SANDBOX_SECRET"),
                System.Environment.GetEnvironmentVariable("PLAID_SANDBOX_ACCESS_TOKEN"), Acklann.Plaid.Environment.Sandbox);
            
        }
        #endregion

        
        #region Properties
        public PlaidClient Client {get; }
        #endregion

        #region Methods
        public async Task<Acklann.Plaid.Entity.Account[]> GetAccounts()
        {
            var a = await Client.FetchAccountAsync(new GetAccountRequest());
            if (a.IsSuccessStatusCode)
            {
                return a.Accounts;
            }
            else
            {
                return Array.Empty<Acklann.Plaid.Entity.Account>();
            }
        }
        #endregion
    }
}