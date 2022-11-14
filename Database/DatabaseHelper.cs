using Dapper;
using System.Data.SqlClient;

namespace TestContainersSQL
{
    public class DatabaseOptions
    {
        public string ConnectionString { get; private set; } = "Data Source=(local);Integrated Security=True";

        public static DatabaseOptions Get()
        {
            var options = new DatabaseOptions();

            var conStr = Environment.GetEnvironmentVariable(Constants.EnvIntegrationTestDatabaseConnectionString);
            if (!string.IsNullOrWhiteSpace(conStr))
            {
                options.ConnectionString = conStr;
            }

            return options;
        }
    }

    public class DatabaseHelper
    {
        public const string SnapshotSuffix = "_Snapshot";

        public string ConnectionString { get; private set; }
        public string Schema { get; } = "dbo";

        public DatabaseHelper(DatabaseOptions databaseOptions)
        {
            var connectionString = new SqlConnectionStringBuilder(databaseOptions.ConnectionString)
            {
                ConnectRetryCount = 5,
                ConnectRetryInterval = 10,
                ConnectTimeout = 60 // in seconds
            };
            ConnectionString = connectionString.ConnectionString;
        }

        public DatabaseHelper CreateNewDatabase(string databaseName)
        {
            var neutralConStr = ConnectionString;
            var conStrBuilder = new SqlConnectionStringBuilder(ConnectionString) { InitialCatalog = databaseName };
            ConnectionString = conStrBuilder.ConnectionString;

            using (var con = new SqlConnection(neutralConStr))
            {
                con.CreateDatabase(databaseName, null, null, Schema);
                con.Execute($"ALTER DATABASE {databaseName} SET ENABLE_BROKER;");
            }

            return this;
        }

        private string GetSnapshotName(string databaseName = "")
        {
            var conStr = new SqlConnectionStringBuilder(ConnectionString);
            var dbName = string.IsNullOrWhiteSpace(databaseName) ? conStr.InitialCatalog : databaseName;
            return dbName + SnapshotSuffix;
        }

        public void Migrate()
        {
            for (int i = 0; i < 5; i++)
            {
                CreateTable($"Table{i}");
            }
        }

        public void CreateTable(string tableName)
        {
            using (var con = new SqlConnection(ConnectionString))
            { 
                con.Execute($@"
                                IF NOT EXISTS (SELECT NULL FROM [INFORMATION_SCHEMA].[TABLES] WHERE TABLE_NAME = '{tableName}' AND TABLE_SCHEMA = 'dbo')
                                CREATE TABLE [dbo].[{tableName}](
                                    [Id] INT NOT NULL,
                                    CONSTRAINT [PK_{tableName}] PRIMARY KEY CLUSTERED ([Id])
                                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                ) ON [PRIMARY]
                                ", Constants.LongRunningCommandTimeoutInSeconds);
            }
        }

        public int Execute(string sql)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                return con.Execute(sql);
            }
        }

        public DatabaseHelper DropDatabaseIfExists()
        {
            return DropDatabaseIfExists(new SqlConnectionStringBuilder(ConnectionString).InitialCatalog);
        }

        public DatabaseHelper DropDatabaseIfExists(string databaseName)
        {
            var conStr = new SqlConnectionStringBuilder(ConnectionString);
            conStr.InitialCatalog = "";
            using (var con = new SqlConnection(conStr.ConnectionString))
            {
                try
                {
                    con.Execute($@"
                        IF DB_ID('{databaseName}') IS NOT NULL 
                            ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;",
                        commandTimeout: Constants.LongRunningCommandTimeoutInSeconds);
                }
                catch (Exception ex)
                {
                    TestContext.Progress.WriteLine(
                        $"Warning: Failed to set single user mode on database '{databaseName}'. Exception: {ex.Message}");
                }

                con.Execute($"DROP DATABASE IF EXISTS [{GetSnapshotName(databaseName)}];",
                    commandTimeout: Constants.LongRunningCommandTimeoutInSeconds);

                con.Execute($"DROP DATABASE IF EXISTS [{databaseName}];",
                    commandTimeout: Constants.LongRunningCommandTimeoutInSeconds);
            }
            return this;
        }
    }
}
