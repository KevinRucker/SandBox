namespace Minimal.Interfaces
{
    /// <summary>
    /// Interface for classes implementing ConnectionString provider functionality
    /// </summary>
    public interface IDBConnectionStringProvider
    {
        /// <summary>
        /// Retrieve the ConnectionString
        /// </summary>
        /// <returns><code>System.String</code> The retrieved ConnectionString</returns>
        string GetConnectionString();
    }
}
