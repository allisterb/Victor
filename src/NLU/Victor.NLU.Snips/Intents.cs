using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Victor
{
    public class Intents
    {
        public Intents(string[] scores, IntentEntity[] entities)
        {
            Scores = scores.Select(s => s.Split(':')).Select(s => new Tuple<string, double>(s[0], Double.Parse(s[1]))).ToArray();
            Entities = entities;
        }

        public Tuple<string, double> Top => Scores.First();

        public Tuple<string, double>[] Scores { get; }

        public IntentEntity[] Entities { get; }
    }
}
