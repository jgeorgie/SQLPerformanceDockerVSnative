using System;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;

namespace TestContainersSQL.TestHelpers.Configuration
{
    public class MsSqlTestContainerDatabaseConfiguration : TestcontainerDatabaseConfiguration
    {
        private const int MsSqlPort = 1433;

        public MsSqlTestContainerDatabaseConfiguration(string image, int port = 0) : base(image, MsSqlPort, port)
        {
            Environments["ACCEPT_EULA"] = "Y";
            Password = "<YourStrong!Passw0rd>";
        }

        /// <inheritdoc />
        public override string Username
        {
            get => "sa";
            set => throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override string Password
        {
            get => Environments["SA_PASSWORD"];
            set => Environments["SA_PASSWORD"] = value;
        }

        /// <inheritdoc />
        public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
            .UntilCommandIsCompleted("/opt/mssql-tools/bin/sqlcmd",
                "-S", $"localhost,{DefaultPort}",
                "-U", Username,
                "-P", Password);
    }
}
