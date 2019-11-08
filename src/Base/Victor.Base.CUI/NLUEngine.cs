using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Victor
{
    public abstract class NLUEngine : Api
    {
        #region Constructors
        public NLUEngine(CancellationToken ct) : base(ct) { }

        public NLUEngine() : this(Api.Ct) { }
        #endregion

        #region Abstract methods
        public abstract Intent GetIntent(string input);
        #endregion
    }
}
