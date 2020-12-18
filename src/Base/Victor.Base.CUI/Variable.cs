using System;
using System.Collections.Generic;
using System.Text;

namespace Victor.CUI
{
    public class Variable
    {
        public Variable(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
