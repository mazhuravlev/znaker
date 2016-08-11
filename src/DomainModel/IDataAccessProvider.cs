using System;
using DomainModel.Entities;

namespace DomainModel
{
    public interface IDataAccessProvider
    {
        void AddTestEntity(TestEntity entity);
        int CountTestEntities();
    }
}
