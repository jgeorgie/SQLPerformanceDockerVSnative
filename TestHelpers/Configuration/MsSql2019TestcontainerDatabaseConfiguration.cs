namespace TestContainersSQL.TestHelpers.Configuration
{
    public sealed class MsSql2019TestcontainerDatabaseConfiguration : MsSqlTestContainerDatabaseConfiguration
    {
        private const string MsSqlImage = "mcr.microsoft.com/mssql/server:2019-CU11-ubuntu-20.04";

        public MsSql2019TestcontainerDatabaseConfiguration(int port = 0) : base(MsSqlImage, port)
        {
        }
    }
}
