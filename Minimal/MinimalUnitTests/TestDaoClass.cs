using Minimal.Common;
using Minimal.Custom_Exceptions;
using Minimal.Data;
using Minimal.Framework.BaseClasses;
using Minimal.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace MinimalUnitTests
{
    public class DepartmentDao : DaoBase<Department>, IDisposable
    {
        private IDBContext _context;
        private SqlDirect _sqlDirect;
        private IDbConnection _connection;
        private bool _disposed = default(bool);

        public DepartmentDao()
        {
            _context = DBContext.Factory(
                    DataProviders.SqlServer,
                    DBConfigFileCSProvider.Factory("DevelopmentAdventureWorks", true));
            _sqlDirect = SqlDirect.Factory(_context);
            _connection = _sqlDirect.GetConnection();
        }

        public override Department Fetch(int Id)
        {
            try
            {
                var entity = new Department();
                var query = 
                    @"SELECT 
                        DepartmentID, 
                        Name, 
                        GroupName, 
                        ModifiedDate 
                    FROM 
                        HumanResources.Department 
                    WHERE 
                        DepartmentID = @DepartmentID";
                var parameters = new List<IDbDataParameter>
                    {
                        _context.CreateParameter(DbType.Int32, ParameterDirection.Input, "@DepartmentID", Id)
                    };
                using (var reader = _sqlDirect.DataReader(query, CommandType.Text, parameters, ref _connection))
                {
                    if (reader.Read())
                    {
                        entity.Load(reader);
                    }
                    else
                    {
                        throw new DBException("DepartmentDao::Fetch Record not found for Id [" + Id.ToString() + "].");
                    }
                }
                return entity;
            }
            catch (Exception ex)
            {
                throw new DBException("DepartmentDao::Fetch Exception fetching record.", ex);
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed) { _connection.Close(); }
            }
        }

        public override IList<Department> FetchList(object filter)
        {
            try
            {
                var entityList = new List<Department>();
                var query = 
                    @"SELECT 
                        DepartmentID, 
                        Name, 
                        GroupName, 
                        ModifiedDate 
                    FROM 
                        HumanResources.Department 
                    WHERE 
                        GroupName = @GroupName";
                var parameters = new List<IDbDataParameter>
                {
                    _context.CreateParameter(DbType.String, ParameterDirection.Input, "@GroupName", filter)
                };
                using (var reader = _sqlDirect.DataReader(query, CommandType.Text, parameters, ref _connection))
                {
                    while (reader.Read())
                    {
                        var entity = new Department();
                        entity.Load(reader);
                        entityList.Add(entity);
                    }
                }
                return entityList;
            }
            catch (Exception ex)
            {
                throw new DBException("DepartmentDao::FetchList Exception fetching records.", ex);
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed) { _connection.Close(); }
            }
        }

        public override IList<Department> FetchAll()
        {
            try
            {
                var entityList = new List<Department>();
                var query = 
                    @"SELECT 
                        DepartmentID, 
                        Name, 
                        GroupName, 
                        ModifiedDate 
                    FROM 
                        HumanResources.Department";
                using (var reader = _sqlDirect.DataReader(query, CommandType.Text, null, ref _connection))
                {
                    while (reader.Read())
                    {
                        var entity = new Department();
                        entity.Load(reader);
                        entityList.Add(entity);
                    }
                }
                return entityList;
            }
            catch (Exception ex)
            {
                throw new DBException("DepartmentDao::FetchAll Exception fetching records.", ex);
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed) { _connection.Close(); }
            }
        }

        public override Department Save(Department entity)
        {
            try
            {
                var query = default(string);
                var parameters = new List<IDbDataParameter>();
                var Id = default(int);
                if (entity.Id.HasValue && entity.IsChanged)
                {
                    // Update
                    Id = entity.Id.Value;
                    parameters.Add(_context.CreateParameter(DbType.Int16, ParameterDirection.Input, "@DepartmentID", Id));
                    parameters.Add(_context.CreateParameter(DbType.String, ParameterDirection.Input, "@Name", entity.Name));
                    parameters.Add(_context.CreateParameter(DbType.String, ParameterDirection.Input, "@GroupName", entity.GroupName));
                    query = 
                        @"UPDATE 
                              HumanResources.Department 
                          SET 
                              Name = @Name, 
                              GroupName = @GroupName, 
                              ModifiedDate = GETDATE() 
                          WHERE 
                              DepartmentID = @DepartmentID";

                    _sqlDirect.NonQuery(query, CommandType.Text, parameters, ref _connection);
                }
                else if (entity.IsChanged)
                {
                    // Insert
                    parameters.Add(_context.CreateParameter(DbType.String, ParameterDirection.Input, "@Name", entity.Name));
                    parameters.Add(_context.CreateParameter(DbType.String, ParameterDirection.Input, "@GroupName", entity.GroupName));
                    query = 
                        @"DECLARE @IDReturn TABLE (ID SMALLINT); 
                          INSERT INTO HumanResources.Department 
                              (Name, GroupName) 
                          OUTPUT 
                              INSERTED.DepartmentID INTO @IDReturn 
                          VALUES 
                              (@Name, @GroupName); 
                          SELECT ID FROM @IDReturn";

                    Id = _sqlDirect.Scalar<short>(query, CommandType.Text, parameters, ref _connection);
                }

                entity = Fetch(Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new DBException("DepartmentDao::Save Exception saving record.", ex);
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed) { _connection.Close(); }
            }
        }

        public override void Delete(Department entity)
        {
            try
            {
                if (!entity.Id.HasValue)
                {
                    return;
                }
                var query = 
                    @"DELETE FROM 
                        HumanResources.Department 
                    WHERE 
                        DepartmentID = @DepartmentID";
                var parameters = new List<IDbDataParameter>
                {
                    _context.CreateParameter(DbType.Int16, ParameterDirection.Input, "@DepartmentID", entity.Id.Value)
                };
                _sqlDirect.NonQuery(query, CommandType.Text, parameters, ref _connection);
            }
            catch (Exception ex)
            {
                throw new DBException("DepartmentDao::Delete Exception saving record.", ex);
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed) { _connection.Close(); }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_connection.State != ConnectionState.Closed) { _connection.Close(); }
                    _connection.Dispose();
                    _connection = null;
                }
                _disposed = true;
            }
        }
    }
}
