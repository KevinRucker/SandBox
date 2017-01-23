using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minimal.Common;
using Minimal.Data;
using Minimal.Interfaces;
using System.Data;

namespace MinimalUnitTests
{
    [TestClass]
    public class DBContextUnitTests
    {
        private IDBConnectionStringProvider csp;

        [TestInitialize]
        public void init()
        {
            csp = DBConfigFileCSProvider.Factory("DevelopmentAdventureWorks", true);
        }

        [TestMethod]
        public void DBContext_ConstructorTest()
        {
            var context = DBContext.Factory(DataProviders.SqlServer, csp);
            Assert.IsNotNull(context);
        }

        [TestMethod]
        public void DBContext_CreateConnectionTest()
        {
            var context = DBContext.Factory(DataProviders.SqlServer, csp);
            using (var conn = context.CreateConnection())
            {
                conn.Open();
                Assert.IsTrue(conn.State == ConnectionState.Open);
            }
        }

        [TestMethod]
        public void DBContext_CreateCommandTest()
        {
            var sql = "SELECT COUNT(*) FROM Production.Product";
            var context = DBContext.Factory(DataProviders.SqlServer, csp);
            var ds = new DataSet();
            using (var connection = context.CreateConnection())
            {
                using (var command = context.CreateCommand(sql, CommandType.Text, connection))
                {
                    var adapter = context.CreateAdapter(command);
                    adapter.Fill(ds);
                    Assert.IsTrue(ds.Tables[0].Rows.Count > 0);
                }
            }
        }

        [TestMethod]
        public void DBContext_CreateDataReaderTest()
        {
            var sql = "SELECT COUNT(*) FROM Production.Product";
            var context = DBContext.Factory(DataProviders.SqlServer, csp);
            using (var connection = context.CreateConnection())
            {
                using (var command = context.CreateCommand(sql, CommandType.Text, connection))
                {
                    using (var reader = context.CreateDataReader(command))
                    {
                        Assert.IsTrue(reader.Read());
                    }
                }
            }
        }
    }
}
