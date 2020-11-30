using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Victor
{
    public abstract class ASREngine : Api
    {
        #region Constructors
        public ASREngine(CancellationToken ct) : base(ct) { }

        public ASREngine() : this(Api.Ct) { }
        #endregion

        #region Abstract methods
        public abstract bool AddGrammar(Dictionary<string, object> entries);
        public abstract bool RemoveGrammar(Dictionary<string, object> entries);
        #endregion
    }
}
