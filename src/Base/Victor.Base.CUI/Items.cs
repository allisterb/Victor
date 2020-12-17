using System;
using System.Collections.Generic;
using System.Text;

namespace Victor.CUI
{
    public class Items: List<object>
    {
        public string Name { get; set; }

        public Type Type { get; set; }

        public int PageSize { get; set; }
        
        public int Page { get; set; }

        public int Selection { get; set; }

        public Action<Intent> ListHandler { get; set; }

        public Action<Intent> DescriptionHandler { get; set; }

        public T Get<T>(int index) => (T)this[index]; 

    }
}
