using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostgreSqlProvider.Entities.v4
{
    public class Contact
    {
        public enum ContactTypes
        {
            Phone,
            Email,
            Skype
        }

        [Key]
        public int Id { get; set; }

        public string Identity { get; set; }
        public int ContactType { get; set; }
        public DateTime CreatedOn { get; set; }

        [InverseProperty("Contact")]
        public List<EntryContact> EntryContacts { get; set; } = new List<EntryContact>();
    }
}