using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Victor
{
    public class Intents
    {
        public Intents(string[] scores, IntentEntity[] entities, string input)
        {
            Scores = scores.Select(s => s.Split(':')).Select(s => new Tuple<string, double>(s[0], Double.Parse(s[1]))).ToArray();
            Entities = entities;
            Input = input;
        }

        public string Input { get; }

        public Tuple<string, double> Top => Scores.First();

        public Tuple<string, double>[] Scores { get; }

        public bool IsNone => Scores.First().Item1 == "None";

        public IntentEntity[] Entities { get; }
    }
}
