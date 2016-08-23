using System.ComponentModel.DataAnnotations;

namespace PostgreSqlProvider.Entities.v4
{
    public class ContactType
    {
        [Key]
        public int Id { get; set; }
    }
}