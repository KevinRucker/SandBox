using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minimal.Common;
using Minimal.Data;
using Minimal.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace MinimalUnitTests
{
    [TestClass]
    public class SqlDirectUnitTest
    {
        private IDBContext _context;
        private SqlDirect _sqlHelper;

        [TestInitialize]
        public void init()
        {
            _context = DBContext.Factory(
                    DataProviders.SqlServer,
                    DBConfigFileCSProvider.Factory("DevelopmentAdventureWorks", true));
            _sqlHelper = SqlDirect.Factory(_context);
        }

        [TestMethod]
        public void SqlDirect_ScalarTest()
        {
            var query = 
                @"SELECT 
                    COUNT(*) 
                FROM 
                    Production.Product";
            var count = _sqlHelper.Scalar<int>(query, CommandType.Text, null);
            Assert.IsTrue(count > 0);
        }

        [TestMethod]
        public void SqlDirect_DataSetTest()
        {
            var query = 
                @"SELECT 
                    * 
                FROM 
                    Production.Product";
            var ds = _sqlHelper.DataSet(query, CommandType.Text, null);
            Assert.IsTrue(ds.Tables.Count > 0, "No tables were returned.");
            Assert.IsTrue(ds.Tables[0].Rows.Count > 0, "No rows were returned.");
        }

        [TestMethod]
        public void SqlDirect_DataReaderTest()
        {
            var query = 
                @"SELECT 
                    * 
                FROM 
                    Production.Product";
            var connection = _sqlHelper.GetConnection();
            try
            {
                using (var reader = _sqlHelper.DataReader(query, CommandType.Text, null, ref connection))
                {
                    Assert.IsTrue(reader.Read());
                }
            }
            finally
            {
                if (connection.State != ConnectionState.Closed) { connection.Close(); }
                connection.Dispose();
                connection = null;
            }
        }

        [TestMethod]
        public void SqlDirect_StoredProcedureTest()
        {
            var sproc = "uspGetBillOfMaterials";
            var parameters = new List<IDbDataParameter>
                {
                    _context.CreateParameter(DbType.Int32, ParameterDirection.Input, "@StartProductId", 803),
                    _context.CreateParameter(DbType.DateTime, ParameterDirection.Input, "@CheckDate", new System.DateTime(2006, 1, 1))
                };
            var result = _sqlHelper.DataSet(sproc, CommandType.StoredProcedure, parameters);
            Assert.IsTrue(result.Tables[0].Rows.Count > 0);
        }

        [TestMethod]
        public void SqlDirect_ParameterizedQueryTest()
        {
            var query = 
                @"SELECT 
                    ProductID 
                FROM 
                    Production.Product 
                WHERE 
                    Name = @Name AND 
                    ProductNumber = @ProductNumber";
            var parameters = new List<IDbDataParameter>
                {
                    _context.CreateParameter(DbType.String, ParameterDirection.Input, "@Name", "Adjustable Race"),
                    _context.CreateParameter(DbType.String, ParameterDirection.Input, "@ProductNumber", "AR-5381")
                };
            var result = _sqlHelper.Scalar<int>(query, CommandType.Text, parameters);
            Assert.IsTrue(result > 0);
        }
    }
}
