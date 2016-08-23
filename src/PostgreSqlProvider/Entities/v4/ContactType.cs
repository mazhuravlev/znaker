using System.ComponentModel.DataAnnotations.Schema;

namespace PostgreSqlProvider.Entities.v4
{
    public class ContactType
    {
        public enum Types
        {
            Phone,
            Email,
            Skype
        }
        public int Id { get; set; }
    }
}