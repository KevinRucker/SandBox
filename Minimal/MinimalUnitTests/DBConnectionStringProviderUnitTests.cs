using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minimal.Data;

namespace MinimalUnitTests
{
    [TestClass]
    public class DBConnectionStringProviderUnitTests
    {
        private string origin = "Server=Kevin-PC;Initial Catalog=AdventureWorks2012;Integrated Security=SSPI;";

        [TestInitialize]
        public void TestInit()
        {
            origin = TestDataFactory.GetDBConnectionString("LocalAdventureWorks", false);
        }

        [TestMethod]
        public void DBConnectionStringProvider_ConstructorTest()
        {
            var csp = DBConfigFileCSProvider.Factory("LocalAdventureWorks", false);
            Assert.IsNotNull(csp);
        }

        [TestMethod]
        public void DBConnectionStringProvider_GetConnectionStringTest()
        {
            var csp = DBConfigFileCSProvider.Factory("LocalAdventureWorks", false);
            var target = csp.GetConnectionString();
            Assert.AreEqual(origin, target);
        }

        [TestMethod]
        public void DBConnectionStringProvider_GetEncryptedConnectionStringTest()
        {
            var csp = DBConfigFileCSProvider.Factory("DevelopmentAdventureWorks", true);
            var target = csp.GetConnectionString();
            Assert.AreEqual(origin, target);
        }
    }
}
