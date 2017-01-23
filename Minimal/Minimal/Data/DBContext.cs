// Author: Kevin Rucker
// License: BSD 3-Clause
// Copyright (c) 2014 - 2015, Kevin Rucker
// All rights reserved.

// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice,
//    this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//
// 3. Neither the name of the copyright holder nor the names of its contributors
//    may be used to endorse or promote products derived from this software without
//    specific prior written permission.
//
// Disclaimer:
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using Minimal.Common;
using Minimal.Custom_Exceptions;
using Minimal.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Minimal.Data
{
    /// <summary>
    /// Concrete implementation of DBContext class
    /// </summary>
    public class DBContext : IDBContext
    {
        private DataProviders _provider;
        private IDBConnectionStringProvider _CSProvider;

        /// <summary>
        /// Private constructor creates an instance of the DBContext class
        /// </summary>
        /// <param name="provider"><code>Minimal.Data.DataProviders</code> Data provider factory to use</param>
        /// <param name="CSProvider">ConnectionString provider to use</param>
        private DBContext(DataProviders provider, IDBConnectionStringProvider CSProvider)
        {
            _provider = provider;
            _CSProvider = CSProvider;
        }

        /// <summary>
        /// Static object factory method
        /// </summary>
        /// <param name="provider"><code>Minimal.Data.DataProviders</code> Data provider factory to use</param>
        /// <param name="CSProvider">ConnectionString provider to use</param>
        /// <returns><code>IDBContext</code> instance</returns>
        public static IDBContext Factory(DataProviders provider, IDBConnectionStringProvider CSProvider)
        {
            return new DBContext(provider, CSProvider);
        }

        /// <summary>
        /// Create a database connection
        /// </summary>
        /// <returns><code>System.Data.IDbConnection</code></returns>
        public IDbConnection CreateConnection()
        {
            try
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(GetProviderName());
                IDbConnection connection = factory.CreateConnection();
                connection.ConnectionString = _CSProvider.GetConnectionString();
                return connection;
            }
            catch (Exception ex)
            {
                throw new DBException("DBContext::CreateConnection unable to create DB Connection object.", ex);
            }
        }

        /// <summary>
        /// Create a database command object
        /// </summary>
        /// <param name="connection"><code>System.Data.IDbConnection</code> to use</param>
        /// <returns><code>System.Data.IDbCommand</code></returns>
        public IDbCommand CreateCommand(IDbConnection connection)
        {
            try
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(GetProviderName());
                IDbCommand command = factory.CreateCommand();
                if (connection == null) { connection = CreateConnection(); }
                if (connection.State != ConnectionState.Open) { connection.Open(); }
                command.Connection = connection;
                return command;
            }
            catch (Exception ex)
            {
                throw new DBException("DBContext::CreateCommand unable to create DB Command object.", ex);
            }
        }

        /// <summary>
        /// Create a database command object
        /// </summary>
        /// <param name="commandText"><code>System.String</code> containing the command text to execute.</param>
        /// <param name="commandType"><code>CommandType</code> enumeration indicating the type of command to execute</param>
        /// <param name="connection"><code>System.Data.IDbConnection</code> to use</param>
        /// <returns><code>System.Data.IDbCommand</code></returns>
        public IDbCommand CreateCommand(string commandText, CommandType commandType, IDbConnection connection)
        {
            try
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(GetProviderName());
                IDbCommand command = factory.CreateCommand();
                command.CommandText = commandText;
                command.CommandType = commandType;
                if (connection == null) { connection = CreateConnection(); }
                if (connection.State != ConnectionState.Open) { connection.Open(); }
                command.Connection = connection;
                return command;
            }
            catch (Exception ex)
            {
                throw new DBException("DBContext::CreateCommand unable to create DB Command object.", ex);
            }
        }

        /// <summary>
        /// Create Parameter
        /// </summary>
        /// <param name="type"></param>
        /// <param name="direction"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IDbDataParameter CreateParameter(DbType type, ParameterDirection direction, string name, object value)
        {
            return CreateParameter(type, direction, name, null, null, null, null, DataRowVersion.Proposed, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="direction"></param>
        /// <param name="name"></param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        /// <param name="size"></param>
        /// <param name="sourceColumn"></param>
        /// <param name="version"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IDbDataParameter CreateParameter(DbType type, ParameterDirection direction, string name, byte? precision, byte? scale, int? size, string sourceColumn, DataRowVersion version, object value)
        {
            try
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(GetProviderName());
                IDbDataParameter parameter = factory.CreateParameter();
                parameter.DbType = type;
                parameter.Direction = direction;
                parameter.ParameterName = name;
                if (precision != null) { parameter.Precision = precision.Value; }
                if (scale != null) { parameter.Scale = scale.Value; }
                if (size != null) { parameter.Size = size.Value; }
                if (!string.IsNullOrEmpty(sourceColumn)) { parameter.SourceColumn = sourceColumn; }
                parameter.SourceVersion = version;
                parameter.Value = value;
                return parameter;
            }
            catch (Exception ex)
            {
                throw new DBException("DBContext::CreateParameter unable to create DB Parameter object.", ex);
            }
        }

        /// <summary>
        /// Add a list of <code>System.Data.IDbParameter</code> to the <code>System.Data.IDbCommand</code>
        /// </summary>
        /// <param name="command"><code>System.Data.IDbCommand</code> to use</param>
        /// <param name="parameters"><code>System.Collections.Generic.IList&lt;System.Data.IDbParameter&gt;</code> to add</param>
        public void AddParameterList(ref IDbCommand command, IList<IDbDataParameter> parameters)
        {
            foreach(IDbDataParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
        }

        /// <summary>
        /// Create a DataAdapter
        /// </summary>
        /// <param name="command"><code>System.Data.IDbCommand</code> to use</param>
        /// <returns><code>System.Data.IDbDataAdapter</code></returns>
        public IDbDataAdapter CreateAdapter(IDbCommand command)
        {
            try
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(GetProviderName());
                IDbDataAdapter adapter = factory.CreateDataAdapter();
                adapter.SelectCommand = command;
                return adapter;
            }
            catch (Exception ex)
            {
                throw new DBException("DBContext::CreateAdapter unable to create DB Adapter object.", ex);
            }
        }

        /// <summary>
        /// Create a DataReader
        /// </summary>
        /// <param name="command"><code>System.Data.IDbCommand</code> to use</param>
        /// <returns><code>System.Data,IDataReader</code></returns>
        public IDataReader CreateDataReader(IDbCommand command)
        {
            try
            {
                return command.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw new DBException("DBContext::CreateDataReader unable to create DataReader.", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetProviderName()
        {
            switch (_provider)
            {
                case DataProviders.Odbc:
                    return "System.Data.Odbc";
                case DataProviders.OleDb:
                    return "System.Data.OleDb";
                case DataProviders.OracleClient:
                    return "System.Data.OracleClient";
                case DataProviders.ODPManaged:
                case DataProviders.ODPUnmanaged:
                    return "Oracle.DataAccess.Client.OracleClientFactory";
                case DataProviders.FireBird:
                    return "FirebirdSql.Data.FirebirdClient";
                case DataProviders.PostgreSQL:
                    return "Npgsql.NpgsqlFactory";
                case DataProviders.MySQL:
                    return "MySql.Data.MySqlClient";
                case DataProviders.SqlServer:
                    return "System.Data.SqlClient";
                default:
                    throw new DBException("DBContext::GetProviderName Unrecognized provider value.");
            }
        }
    }
}
