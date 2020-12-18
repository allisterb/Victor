using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Victor.CUI
{
    public class Items: List<object>
    {
        public Items(string name, Type type, Action<Intent> listHandler, Action<int> descriptionHandler, int pageSize = 8)
        {
            Name = name;
            Type = type;
            PageSize = pageSize;
            ListHandler = listHandler;
            DescriptionHandler = descriptionHandler;
        }

        public string Name { get; set; }

        public Type Type { get; set; }

        public int PageSize { get; set; }
        
        public int Page { get; set; }

        public int Selection { get; set; }

        public Action<Intent> ListHandler { get; set; }

        public Action<int> DescriptionHandler { get; set; }

        public void Add<T>(IEnumerable<T> items)
        {
            this.AddRange(items.Cast<object>());
            this.Page = 1;
        }
        public T Get<T>(int index) => (T)this[index]; 

    }
}
