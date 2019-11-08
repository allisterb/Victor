using System;
using System.Collections.Generic;
using System.Text;

namespace Victor
{
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
}
