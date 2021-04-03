using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

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
                configureContext: context => { context.IsAtty = true; },
                createCustomProvider: mem =>
                {
                    mem.Attach(w => new MemoryAnsiTextWriter(w));
                    return null;
                });
            var session = engine.CreateSession("-f /dev/stdin", InputText);
            Assert.True(session.IsActive);

            var response = session.ExecuteCommand("bal checking --account=code");
            Assert.False(response.HasErrors);
            //Assert.Equal(BalCheckingOutputText.Replace("\r", "").Trim(), response.OutputText.Trim());
        }
    }
}
