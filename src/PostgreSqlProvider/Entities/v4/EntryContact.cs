using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PostgreSqlProvider.Entities.v4
{
    public class EntryContact
    {
        [Key]
        public int Id { get; set; }

        public int ContactId { get; set; }

        [ForeignKey("ContactId")]
        public Contact Contact { get; set; }

        public int EntryId { get; set; }

        [ForeignKey("EntryId")]
        public Entry Entry { get; set; }
    }
}