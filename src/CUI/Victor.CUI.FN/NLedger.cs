using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NLedger.Journals;
using NLedger.Utility.ServiceAPI;

namespace Victor.CUI.FN
{
    public class NLedgerSession : Api
    {
        #region Constructors
        public NLedgerSession(string file)
        {
            if (!File.Exists(file))
            {
                throw new ArgumentException($"The file {file} does not exist.");
            }
            Engine = new ServiceEngine(
                configureContext: context => { context.IsAtty = true; },
                createCustomProvider: mem =>
                {
                    mem.Attach(w => new MemoryAnsiTextWriter(w));
                    return null;
                });
            Session = Engine.CreateSession(file);
            Initialized = true;

        }
        #endregion

        #region Properties
        public ServiceEngine Engine { get; }

        public ServiceSession Session { get; }
        #endregion

        #region Methods
        public IEnumerable<string> GetAccounts() => Session.GlobalScope.Session.Journal.Master.Accounts.Keys;



        #endregion
    }
}
