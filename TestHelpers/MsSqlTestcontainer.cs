using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;

namespace TestContainersSQL
{
    public class MsSqlTestcontainer : TestcontainerDatabase
    {
        internal MsSqlTestcontainer(ITestcontainersConfiguration configuration, ILogger logger) : base(
            configuration, logger)
        {
        }

        /// <inheritdoc />
        public override async Task<ExecResult> ExecuteQueryAsync(string query, CancellationToken cancellation = default)
        {
            return await ExecAsync(
                new[]
                {
                    "/opt/mssql-tools/bin/sqlcmd",
                    "-b",
                    "-r", "1",
                    "-S", $"localhost,{ContainerPort}",
                    "-U", Username,
                    "-P", Password,
                    "-Q", query
                }, cancellation);
        }

        /// <inheritdoc />
        public override async Task<ExecResult> ExecuteQueryAsync(string query, string databaseName,
            CancellationToken cancellation = default)
        {
            return await ExecAsync(
                new[]
                {
                    "/opt/mssql-tools/bin/sqlcmd",
                    "-b",
                    "-r", "1",
                    "-S", $"localhost,{ContainerPort}",
                    "-d", databaseName,
                    "-U", Username,
                    "-P", Password,
                    "-Q", query
                }, cancellation);
        }

        /// <inheritdoc />
        public override async Task<string> CreateDatabaseAsync(string databaseName, CancellationToken cancellation = default)
        {
            var query = $"CREATE DATABASE {databaseName}";
            await ExecuteQueryAsync(query, cancellation);

            return ConnectionString(databaseName);
        }

        /// <inheritdoc />
        public override async Task CreateSchemaAsync(string databaseName, string schemaName,
            CancellationToken cancellation = default)
        {
            var query = $"CREATE SCHEMA {schemaName}";
            await ExecuteQueryAsync(query, databaseName, cancellation);
        }

        /// <inheritdoc />
        public override string ConnectionString(string databaseName, bool encrypt = false) =>
            $"Server={Hostname},{Port};Database={databaseName};User Id={Username};Password={Password};Encrypt={encrypt};";
    }
}
