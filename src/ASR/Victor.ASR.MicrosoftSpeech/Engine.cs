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

        }
        #endregion
        
        #region Properties                                                      
        public SpeechRecognitionEngine SRE = new SpeechRecognitionEngine();
        #endregion
    }
}
