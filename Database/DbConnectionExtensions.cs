using Dapper;
using System.Data.Common;

namespace TestContainersSQL
{
    public static class DbConnectionExtensions
    {
        /// <summary>
        /// Creates a new database along with a schema and optionally a database user (if createUser and password are not null).
        /// </summary>
        public static void CreateDatabase(this DbConnection con, string databaseName, string createUser, string password, string schema)
        {
            if (databaseName == null) throw new ArgumentNullException(nameof(databaseName));
            if (string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentException("databaseName cannot be an empty string.", nameof(databaseName));

            schema = schema ?? "dbo";

            con.Execute($@"
                CREATE DATABASE [{databaseName}]

                ALTER DATABASE[{databaseName}] SET ALLOW_SNAPSHOT_ISOLATION ON

                ALTER DATABASE [{databaseName}] SET READ_COMMITTED_SNAPSHOT ON",
                commandTimeout: Constants.LongRunningCommandTimeoutInSeconds);

            con.Execute($@"
                USE [{databaseName}]

                IF NOT EXISTS (SELECT NULL FROM [INFORMATION_SCHEMA].SCHEMATA WHERE [SCHEMA_NAME] = '{schema}') 
                EXEC('CREATE SCHEMA {schema}')",
                commandTimeout: Constants.LongRunningCommandTimeoutInSeconds);

            if (!string.IsNullOrWhiteSpace(createUser) && !string.IsNullOrWhiteSpace(password))
            {
                con.Execute($@"
                    CREATE LOGIN [{createUser}] WITH PASSWORD='{password}', DEFAULT_DATABASE=[{
                        databaseName
                    }], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF

                    USE [{databaseName}]

                    CREATE USER [{createUser}] FOR LOGIN [{createUser}] WITH DEFAULT_SCHEMA={schema}

                    ALTER ROLE [db_owner] ADD MEMBER[{createUser}]", commandTimeout: Constants.LongRunningCommandTimeoutInSeconds);
            }
        }
    }
}
