using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;

namespace TestContainersSQL
{
    public abstract class TestcontainerDatabase : HostedServiceContainer
    {
        protected TestcontainerDatabase(ITestcontainersConfiguration configuration, ILogger logger) : base(
            configuration, logger)
        {
        }

        /// <summary>
        /// Execute a query in the context of the entire SQL server.
        /// </summary>
        /// <param name="query">The string query to execute</param>
        /// <param name="cancellation">Cancellation token, to abort the query</param>
        /// <returns>The execution result</returns>
        public abstract Task<ExecResult> ExecuteQueryAsync(string query, CancellationToken cancellation = default);

        /// <summary>
        /// Execute a query in the context of a specific database on the SQL
        /// server.
        /// </summary>
        /// <param name="query">The string query to execute</param>
        /// <param name="databaseName">The name of the database against which to execute the query</param>
        /// <param name="cancellation">Cancellation token, to abort the query</param>
        /// <returns>The execution result</returns>
        public abstract Task<ExecResult> ExecuteQueryAsync(string query, string databaseName,
            CancellationToken cancellation = default);

        /// <summary>
        /// Create a new database with the specified name on the SQL server
        /// represented by this container.
        /// </summary>
        /// <param name="databaseName">The name of the database to create</param>
        /// <param name="cancellation">Cancellation token, to abort the database creation</param>
        /// <returns>The connection string for the newly created database</returns>
        public abstract Task<string> CreateDatabaseAsync(string databaseName, CancellationToken cancellation = default);

        /// <summary>
        /// Create a new schema in the database with the speficied name on the
        /// SQL server represented by this container.
        /// </summary>
        /// <param name="databaseName">The name of the database in which to create the schema</param>
        /// <param name="schemaName">The name of the schema to create</param>
        /// <param name="cancellation">Cancellation token, to abort the database creation</param>
        public abstract Task CreateSchemaAsync(string databaseName, string schemaName,
            CancellationToken cancellation = default);

        /// <summary>
        /// Get the connection string for the database with the specified name.
        /// The database does not need to exist on the SQL server in order to
        /// generate a connection string.
        /// </summary>
        /// <param name="databaseName">The name of the database to create</param>
        /// <param name="encrypt">Whether the connection string should enable encryption on the database connection</param>
        /// <returns>The connection string for the specified database</returns>
        public abstract string ConnectionString(string databaseName, bool encrypt = false);
    }
}
