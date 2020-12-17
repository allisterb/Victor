using System;
using System.Collections.Generic;
using System.Text;

namespace Victor.CUI
{
    public class Menu
    {
        public string Name { get; set; }

        public List<string> Items { get; set; }

        public int Selection { get; set; }

        public Action<int> Handler { get; set; }
    }
}
