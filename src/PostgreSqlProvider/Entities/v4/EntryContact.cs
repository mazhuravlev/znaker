using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PostgreSqlProvider.Entities.v4
{
    public class EntryContact
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public Contact Contact { get; set; }
        public int EntryId { get; set; }
        public Entry Entry { get; set; }
    }
}