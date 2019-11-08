using System;
using System.Collections.Generic;
using System.Text;

namespace Victor
{
    public class IntentScore
    {
        public IntentScore(string label, double score)
        {
            Label = label;
            Score = score;
        }
        public string Label { get; }

        public double Score { get; }
    }
}
