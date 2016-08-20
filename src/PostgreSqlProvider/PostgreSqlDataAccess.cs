using System.Linq;
using DomainModel;
using DomainModel.Entities;

namespace PostgreSqlProvider
{
    public class PostgreSqlDataAccess : IDataAccessProvider
    {
        private readonly PostgreSqlContext _db;

        public PostgreSqlDataAccess(PostgreSqlContext db)
        {
            _db = db;
        }

        public void AddTestEntity(TestEntity entity)
        {
            _db.TestEntities.Add(entity);
            _db.SaveChanges();
        }

        public int CountTestEntities()
        {
            return _db.TestEntities.Count();
        }

        public void AddPhone(Phone phone)
        {
            _db.Phones.Add(phone);
            _db.SaveChanges();
        }

        public int CountPhones()
        {
            return _db.Phones.Count();
        }
    }
}
