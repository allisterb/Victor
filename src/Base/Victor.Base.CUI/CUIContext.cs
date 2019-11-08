using System;
using System.Collections.Generic;
using System.Text;

namespace Victor
{
    public class CUIContext
    {
        public CUIContext(DateTime time, string label)
        {
            Time = time;
            Label = label;
        }
        public DateTime Time { get; }

        public string Label;
    }
}
