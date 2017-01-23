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

using Minimal.Custom_Exceptions;
using Minimal.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Minimal.Data
{
    /// <summary>
    /// Helper class for data access
    /// </summary>
    public class SqlDirect
    {
        private IDBContext _context;

        /// <summary>
        /// 
        /// </summary>
        private SqlDirect(IDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SqlDirect Factory(IDBContext context)
        {
            return new SqlDirect(context);
        }

        /// <summary>
        /// Query the database and return a DataSet
        /// </summary>
        /// <param name="sql"><code>string</code> containing the SQL to execute</param>
        /// <param name="commandType"><code>CommandType</code> enumeration indicating the type of command to execute</param>
        /// <param name="parameters"><code>List</code> of <code>SqlParameters</code>, null if none</param>
        /// <returns><see cref="System.Data.DataSet"/></returns>
        public DataSet DataSet(string sql, CommandType commandType, IList<IDbDataParameter> parameters)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    using (var command = _context.CreateCommand(sql, commandType, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }
                        var adapter = _context.CreateAdapter(command);
                        var returnValue = new DataSet();
                        adapter.Fill(returnValue);
                        return returnValue;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DBException("SqlDirect::DataSet Unable to execute query.", ex);
            }
        }

        /// <summary>
        /// Query the database and return a DataSet using a provided connection
        /// </summary>
        /// <param name="sql"><code>string</code> containing the SQL to execute</param>
        /// <param name="commandType"><code>CommandType</code> enumeration indicating the type of command to execute</param>
        /// <param name="parameters"><code>List</code> of <code>SqlParameters</code>, null if none</param>
        /// <param name="connection"><see cref="IDbConnection"/> to use for the operation - the connection will remain open afterwards</param>
        /// <returns><see cref="System.Data.DataSet"/></returns>
        /// <remarks>
        /// Since the database connection remains open, the caller is responsible for closing and properly disposing of the connection
        /// </remarks>
        public DataSet DataSet(string sql, CommandType commandType, IList<IDbDataParameter> parameters, ref IDbConnection connection)
        {
            try
            {
                using (var command = _context.CreateCommand(sql, commandType, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    var adapter = _context.CreateAdapter(command);
                    var returnValue = new DataSet();
                    adapter.Fill(returnValue);
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                throw new DBException("SqlDirect::DataSet Unable to execute query.", ex);
            }
        }

        /// <summary>
        /// Query the database and return an <code>IDataReader</code> using a provided connection
        /// </summary>
        /// <param name="sql"><code>string</code> containing the sql to execute</param>
        /// <param name="commandType"><code>CommandType</code> enumeration indicating the type of command to execute</param>
        /// <param name="parameters"><code>List</code> of <code>SqlParameters</code>, null if none</param>
        /// <param name="connection"><code>IDbConnection</code> connection to database</param>
        /// <returns><see cref="System.Data.IDataReader"/></returns>
        /// <remarks>
        /// Since a DataReader requires that the database connection remain open
        /// for the lifetime of the DataReader, the caller is responsible for closing
        /// and properly disposing of the connection
        /// </remarks>
        public IDataReader DataReader(string sql, CommandType commandType, IList<IDbDataParameter> parameters, ref IDbConnection connection)
        {
            try
            {
                using (IDbCommand command = _context.CreateCommand(sql, commandType, connection))
                {
                    if (parameters != null)
                    {
                        foreach (IDbDataParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    return _context.CreateDataReader(command);
                }
            }
            catch (Exception ex)
            {
                throw new DBException("SqlDirect::DataReader Unable to execute query.", ex);
            }
        }

        /// <summary>
        /// Query the database and return a scalar value
        /// </summary>
        /// <typeparam name="T">The <code>Type</code> of the value to return</typeparam>
        /// <param name="sql"><code>string</code> containing the sql to execute</param>
        /// <param name="commandType"><code>CommandType</code> enumeration indicating the type of command to execute</param>
        /// <param name="parameters"><code>List</code> of <code>SqlParameters</code>, null if none</param>
        /// <returns>Scalar value</returns>
        public T Scalar<T>(string sql, CommandType commandType, IList<IDbDataParameter> parameters)
        {
            try
            {
                using (IDbConnection connection = _context.CreateConnection())
                {
                    using (IDbCommand command = _context.CreateCommand(sql, commandType, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (IDbDataParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }
                        object temp = command.ExecuteScalar();
                        if (temp != null)
                        {
                            return (T)temp;
                        }
                        else
                        {
                            return default(T);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DBException("SqlDirect::Scalar Unable to execute query.", ex);
            }
        }

        /// <summary>
        /// Query the database and return a scalar value using a provided connection
        /// </summary>
        /// <typeparam name="T">The <code>Type</code> of the value to return</typeparam>
        /// <param name="sql"><code>string</code> containing the sql to execute</param>
        /// <param name="commandType"><code>CommandType</code> enumeration indicating the type of command to execute</param>
        /// <param name="parameters"><code>List</code> of <code>SqlParameters</code>, null if none</param>
        /// <param name="connection"><see cref="IDbConnection"/> to use for the operation - the connection will remain open afterwards</param>
        /// <returns>Scalar value</returns>
        /// <remarks>
        /// Since the database connection remains open, the caller is responsible for closing and properly disposing of the connection
        /// </remarks>
        public T Scalar<T>(string sql, CommandType commandType, IList<IDbDataParameter> parameters, ref IDbConnection connection)
        {
            try
            {
                using (IDbCommand command = _context.CreateCommand(sql, commandType, connection))
                {
                    if (parameters != null)
                    {
                        foreach (IDbDataParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    object temp = command.ExecuteScalar();
                    if (temp != null)
                    {
                        return (T)temp;
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DBException("SqlDirect::Scalar Unable to execute query.", ex);
            }
        }

        /// <summary>
        /// Issue a non-query against the database
        /// </summary>
        /// <param name="sql"><code>string</code> containing the SQL to execute</param>
        /// <param name="commandType"><code>CommandType</code> enumeration indicating the type of command to execute</param>
        /// <param name="parameters"><code>List</code> of <code>SqlParameters</code>, null if none</param>
        public void NonQuery(string sql, CommandType commandType, IList<IDbDataParameter> parameters)
        {
            try
            {
                using (IDbConnection connection = _context.CreateConnection())
                {
                    using (IDbCommand command = _context.CreateCommand(sql, commandType, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (IDbDataParameter parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DBException("SqlDirect::NonQuery Unable to execute query.", ex);
            }
        }

        /// <summary>
        /// Issue a non-query against the database using a provided connection
        /// </summary>
        /// <param name="sql"><code>string</code> containing the SQL to execute</param>
        /// <param name="commandType"><code>CommandType</code> enumeration indicating the type of command to execute</param>
        /// <param name="parameters"><code>List</code> of <code>SqlParameters</code>, null if none</param>
        /// <param name="connection"><see cref="IDbConnection"/> to use for the operation - the connection will remain open afterwards</param>
        /// <remarks>
        /// Since the database connection remains open, the caller is responsible for closing and properly disposing of the connection
        /// </remarks>
        public void NonQuery(string sql, CommandType commandType, IList<IDbDataParameter> parameters, ref IDbConnection connection)
        {
            try
            {
                using (IDbCommand command = _context.CreateCommand(sql, commandType, connection))
                {
                    if (parameters != null)
                    {
                        foreach (IDbDataParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new DBException("SqlDirect::NonQuery Unable to execute query.", ex);
            }
        }

        /// <summary>
        /// Gets connection using internal DBContext
        /// </summary>
        /// <returns><see cref="IDbConnection"/></returns>
        public IDbConnection GetConnection()
        {
            return _context.CreateConnection();
        }
    }
}
