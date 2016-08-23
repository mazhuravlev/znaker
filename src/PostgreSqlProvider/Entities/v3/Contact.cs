using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostgreSqlProvider.Entities.v3
{
    //все типы контактов хранить в одной таблице
    //контакт может быть точный, а могут быть и предположительный, в случае есть будет пересечение
    //например по одинаковому мылу - разные телефоны, и оба уже есть (у разных пользователей)
    //и тут использован предположительный
    public class Contact
    {
        public long Id { get; set; }
        public string Identity { get; set; } // <- и вот тут прям все подряд хроним
        public DateTime CreateDateTime { get; set; }
        public ContactType ContactType { get; set; }
        public long ContactTypeId { get; set; }
        public List<EntryInContact> ContactEntries { get; set; } = new List<EntryInContact>();
        public List<UserInContact> ContactUsers { get; set; } = new List<UserInContact>();
    }
}
