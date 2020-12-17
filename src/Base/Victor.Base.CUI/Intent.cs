using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Victor.CUI
{
    public class Intent
    {
        #region Constructors
        public Intent(string input, IEnumerable<IntentScore> scores, IEnumerable<IntentEntity> entities)
        {
            Input = input;
            Scores = scores.ToArray();
            Entities = entities.ToArray();
        }
        #endregion

        #region Abstract properties
        public IntentScore Top => Scores.OrderByDescending(s => s.Score).First();

        public bool IsNone => Scores.Count() == 0 || Top.Label == "None";
        #endregion

        #region Properties
        public string Input { get; }
        
        public IntentScore[] Scores { get; }
        
        public IntentEntity[] Entities { get; }
        #endregion
    }

    public class IntentEntity
    {
        #region Constructor
        public IntentEntity(string rawValue, string value, object[] alternatives, string entity, string kind, string slot)
        {
            RawValue = rawValue;
            Value = value;
            Alternatives = alternatives;
            Entity = entity;
            Kind = kind;
            SlotName = slot;
        }
        #endregion

        #region Properties
        public string RawValue { get; }

        public string Value { get; }

        public object[] Alternatives { get; }

        public string Entity { get; set; }

        public string Kind { get; }

        public string SlotName { get; set; }
        #endregion
    }

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
