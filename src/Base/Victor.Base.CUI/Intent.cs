using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Victor
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

        public bool IsNone => Top.Label == "None";
        #endregion

        #region Properties
        public string Input { get; }
        
        public IntentScore[] Scores { get; }
        
        public IntentEntity[] Entities { get; }
        #endregion
    }
}
