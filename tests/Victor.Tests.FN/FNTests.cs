using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

using NLedger;

using NLedger.Accounts;
using NLedger.Amounts;
using NLedger.Expressions;
using NLedger.Journals;
using NLedger.Textual;
using NLedger.Values;
using NLedger.Xacts;
using NLedger.Abstracts.Impl;
using NLedger.Utility.ServiceAPI;
namespace Victor.Tests
{
    public class FNTests
    {
        private static readonly string InputText = @"
2009/10/29 (XFER) Panera Bread
    Expenses:Food               $4.50
    Assets:Checking
2009/10/30 (DEP) Pay day!
    Assets:Checking            $20.00
    Income
2009/10/30 (XFER) Panera Bread
    Expenses:Food               $4.50
    Assets:Checking
2009/10/31 (559385768438A8D7) Panera Bread
    Expenses:Food               $4.50
    Liabilities:Credit Card
";
        [Fact]
        public void CanStartNLedger()
        {
            var engine = new ServiceEngine(
                configureContext: context => { context.IsAtty = true; context.Logger = new NLedger.Utils.Logger(); },
                createCustomProvider: mem =>
                {
                    mem.Attach(w => new MemoryAnsiTextWriter(w));
                    return null;
                });
            var session = engine.CreateSession("-f ledger.dat");
            var journal = session.GlobalScope.Session.Journal;
            Assert.True(session.IsActive);
           
            var response = session.ExecuteCommand("bal checking --account=code");

            session.ExecuteJournalAction((j) =>
            {
                Xact xact = new Xact();
                xact.AddPost(new Post(journal.Master, new Amount(10, new NLedger.Commodities.Commodity(session.MainApplicationContext.CommodityPool, new NLedger.Commodities.CommodityBase("$")))));
                xact.AddPost(new Post(journal.Master, new Amount(-10, new NLedger.Commodities.Commodity(session.MainApplicationContext.CommodityPool, new NLedger.Commodities.CommodityBase("$")))));
                j.AddXact(xact);
            });

            
            // response = session.ExecuteCommand("bal checking --account=code");
            //session.ExecuteCommand
            //j
            response = session.ExecuteCommand("bal checking --account=code");
            //session.Dispose();
            Assert.False(response.HasErrors);
            Assert.Equal(2, session.GlobalScope.Session.Journal.Xacts.Count);
            //session.ServiceEngine.
            //Assert.Equal(BalCheckingOutputText.Replace("\r", "").Trim(), response.OutputText.Trim());
        }
    }
}
