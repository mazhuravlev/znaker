using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sakura.AspNetCore;

namespace Znaker.Models
{
    public class HomeModel
    {
        public int TotalEntries;
        public IPagedList<Item> Items;

        public class Item
        {
            public string Identity;
            public string Data;
            public int TotalData;
        }
    }
}
