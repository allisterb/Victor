using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Victor
{
    public class RecognitionResult
    {
        public RecognitionResult(string text, double confidence)
        {
            Text = text;
            Confidence = confidence;
        }

        #region Properties
        public string Text { get; }
        
        public double Confidence { get; }
        #endregion
    }

    public delegate void OnRecognitionResult(RecognitionResult result);
}
