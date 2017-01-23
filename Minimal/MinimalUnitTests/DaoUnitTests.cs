using System;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinimalUnitTests
{
    [TestClass]
    public class DaoUnitTests
    {
        [TestMethod]
        public void DaoTest()
        {
            using (var departmentDao = new DepartmentDao())
            {
                // Using transaction to roll back test data once test completes
                using (var txScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    // Fetch entity using Id
                    var entity = departmentDao.Fetch(1);
                    Assert.IsNotNull(entity, "DAO Fetch failed.");

                    // Modify property
                    entity.Name = entity.Name + " Altered";
                    Assert.IsTrue(entity.IsChanged, "Entity IsChanged failed.");

                    // Save modified entity
                    entity = departmentDao.Save(entity);
                    Assert.IsFalse(entity.IsChanged, "DAO Save of modified entity failed.");

                    // Fetch filtered list
                    var entityList = departmentDao.FetchList("Research and Development");
                    Assert.IsTrue(entityList.Count > 0, "DAO FetchList failed.");

                    // Fetch all
                    var allEntities = departmentDao.FetchAll();
                    Assert.IsTrue(allEntities.Count > 0, "DAO FetchAll failed.");

                    // Create new entity and save
                    var testId = Guid.NewGuid().ToString();
                    var newEntity = new Department
                        {
                            GroupName = "Test Group " + testId,
                            Name = "Test " + testId
                        };
                    newEntity = departmentDao.Save(newEntity);
                    Assert.IsTrue(newEntity.Id.HasValue, "DAO Save of new entity failed.");
                }
            }
        }
    }
}
