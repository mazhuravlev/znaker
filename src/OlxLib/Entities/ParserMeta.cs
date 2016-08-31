using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OlxLib.Entities
{
    public class ParserMeta
    {
        public int Id { get; set; }
        public OlxType OlxType { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
