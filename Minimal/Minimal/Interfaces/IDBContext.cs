using System.Collections.Generic;
using System.Data;

namespace Minimal.Interfaces
{
    /// <summary>
    /// Interface for classes implementing DBContext
    /// </summary>
    public interface IDBContext
    {
        /// <summary>
        /// Create a database connection
        /// </summary>
        /// <returns><code>System.Data.IDbConnection</code></returns>
        IDbConnection CreateConnection();
        /// <summary>
        /// Create a database command object
        /// </summary>
        /// <param name="connection"><code>System.Data.IDbConnection</code> to use</param>
        /// <returns><code>System.Data.IDbCommand</code></returns>
        IDbCommand CreateCommand(IDbConnection connection);
        /// <summary>
        /// Create a database command object
        /// </summary>
        /// <param name="commandText"><code>System.String</code> containing the command text to execute.</param>
        /// <param name="commandType"><code>CommandType</code> enumeration indicating the type of command to execute</param>
        /// <param name="connection"><code>System.Data.IDbConnection</code> to use</param>
        /// <returns><code>System.Data.IDbCommand</code></returns>
        IDbCommand CreateCommand(string commandText, CommandType commandType, IDbConnection connection);
        /// <summary>
        /// Create Parameter
        /// </summary>
        /// <param name="type"></param>
        /// <param name="direction"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IDbDataParameter CreateParameter(DbType type, ParameterDirection direction, string name, object value);
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
        IDbDataParameter CreateParameter(DbType type, ParameterDirection direction, string name, byte? precision, byte? scale, int? size, string sourceColumn, DataRowVersion version, object value);
        /// <summary>
        /// Add a list of <code>System.Data.IDbParameter</code> to the <code>System.Data.IDbCommand</code>
        /// </summary>
        /// <param name="command"><code>System.Data.IDbCommand</code> to use</param>
        /// <param name="parameters"><code>System.Collections.Generic.IList&lt;System.Data.IDbParameter&gt;</code> to add</param>
        void AddParameterList(ref IDbCommand command, IList<IDbDataParameter> parameters);
        /// <summary>
        /// Create a DataAdapter
        /// </summary>
        /// <param name="command"><code>System.Data.IDbCommand</code> to use</param>
        /// <returns><code>System.Data.IDbDataAdapter</code></returns>
        IDbDataAdapter CreateAdapter(IDbCommand command);
        /// <summary>
        /// Create a DataReader
        /// </summary>
        /// <param name="command"><code>System.Data.IDbCommand</code> to use</param>
        /// <returns><code>System.Data,IDataReader</code></returns>
        IDataReader CreateDataReader(IDbCommand command);
    }
}
