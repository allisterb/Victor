using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Victor.CUI
{
    public class Menu
    {
        public Menu(string name, Action<int> handler, params string[] items)
        {
            Name = name;
            Items = new List<string>(items);
            Handler = handler;
        }

        public Menu(string name, Action<int> handler, IEnumerable<string> items)
        {
            Name = name;
            Items = items.ToList();
            Handler = handler;
        }
        public string Name { get; set; }

        public List<string> Items { get; set; }

        public int Selection { get; set; }

        public Action<int> Handler { get; set; }
    }
}
