using System;
using DomainModel.Entities;

namespace DomainModel
{
    public interface IDataAccessProvider
    {
        void AddTestEntity(TestEntity entity);
        int CountTestEntities();
        void AddPhone(Phone phone);
        int CountPhones();
    }
}
