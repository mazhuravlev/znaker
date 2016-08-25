using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public Source Source;
        public string Data;
        public string IdAtSource;
        public DateTime CreatedAt;
    }

    public enum Source
    {
        Avito = 1,
        OlxRu = 2
    }
}
