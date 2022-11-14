namespace TestContainersSQL.TestHelpers.Configuration
{
    public sealed class MsSql2017TestcontainerDatabaseConfiguration : MsSqlTestContainerDatabaseConfiguration
    {
        private const string MsSqlImage = "mcr.microsoft.com/mssql/server:2017-CU29-ubuntu-16.04";

        public MsSql2017TestcontainerDatabaseConfiguration(int port = 0) : base(MsSqlImage, port)
        {
        }
    }
}
