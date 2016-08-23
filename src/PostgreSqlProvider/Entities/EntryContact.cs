namespace PostgreSqlProvider.Entities 
{
    public class EntryContact
    {
        public long Id { get; set; }
        public long ContactId { get; set; }
        public Contact Contact { get; set; }
        public long EntryId { get; set; }
        public Entry Entry { get; set; }
    }
}