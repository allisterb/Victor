using System;
using System.Collections.Generic;
using System.Text;

namespace Victor.CUI
{
    public class Context
    {
        public Context(DateTime time, string label, Intent intent = null, Action<Intent> action = null)
        {
            Time = time;
            Label = label;
            Intent = intent;
            IntentAction = action;
        }

        public DateTime Time { get; }

        public string Label;

        public Intent Intent { get; protected set; }
        public Action<Intent> IntentAction { get; protected set; }

        public void SetIntentAction(Intent intent, Action<Intent> action)
        {
            Intent = intent;
            IntentAction = action;
        }
    }
}
