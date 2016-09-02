using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure;
using PostgreSqlProvider.Entities;

namespace MyToPg
{
    public class PhoneItem
    {
        public string Number;
        public DateTime CreatedAt;
        public List<DataItem> DataItems;
    }

    public class DataItem
    {
        public SourceType Source;
        public string Data;
        public string IdAtSource;
        public DateTime CreatedAt;
    }

    
}
