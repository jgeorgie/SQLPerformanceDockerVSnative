using Dapper;
using System.Data.SqlClient;
using System.Diagnostics;

namespace TestContainersSQL
{
    public class SqlPerformanceIT
    {

        protected void StartTest(SqlConnection con)
        {
            for (int i = 0; i < 10; i++)
            {
                con.Execute("EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'", commandTimeout: Constants.LongRunningCommandTimeoutInSeconds);
            }
        }

        [Test]
        public void OnPremSql()
        {
            string databaseName = "OnPremSql__test";

            var databaseHelper = new DatabaseHelper(DatabaseOptions.Get());

            databaseHelper.DropDatabaseIfExists(databaseName);
            databaseHelper.CreateNewDatabase(databaseName);
            databaseHelper.Migrate();

            using (var con = new SqlConnection(databaseHelper.ConnectionString))
            {
                var timer = Stopwatch.StartNew();
                timer.Start();
                StartTest(con);
                timer.Stop();
                Debug.WriteLine($"OnPremSql {timer.ElapsedMilliseconds}");
            }

            Assert.True(true);
        }

        [Test]
        public async Task TestContainerSql()
        {
            string databaseName = "TestContainerSql__test";

            var connectionString = await DockerSetUpFixture.MsSqlContainer.CreateDatabaseAsync(databaseName);
            Environment.SetEnvironmentVariable(Constants.EnvIntegrationTestDatabaseConnectionString, connectionString);
            var databaseHelper = new DatabaseHelper(DatabaseOptions.Get());
            databaseHelper.Migrate();

            using (var con = new SqlConnection(connectionString))
            {
                var timer = Stopwatch.StartNew();
                timer.Start();
                StartTest(con);
                timer.Stop();
                Debug.WriteLine($"TestContainerSql__test {timer.ElapsedMilliseconds}");
            }

            Assert.True(true);
        }
    }
}