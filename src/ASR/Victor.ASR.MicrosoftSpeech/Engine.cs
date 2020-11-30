using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;

namespace Victor
{
    public class MSSpeech : ASREngine
    {
        #region Constructors 
        public MSSpeech() :base()
        {
            //this.SRE.
        }
        #endregion
        
        #region fields.                                                      
        public SpeechRecognitionEngine SRE = new SpeechRecognitionEngine();

        #endregion

        public override bool AddGrammar(Dictionary<string, object> entries)
        {
            throw new NotImplementedException();
        }

        public override bool RemoveGrammar(Dictionary<string, object> entries)
        {
            throw new NotImplementedException();
        }
    }
}
