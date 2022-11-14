using DotNet.Testcontainers.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestContainersSQL.TestHelpers;
using TestContainersSQL.TestHelpers.Configuration;

namespace TestContainersSQL
{
    [SetUpFixture]
    public class DockerSetUpFixture
    {
        private static MsSqlTestcontainer? _msSqlContainer;

        internal static MsSqlTestcontainer MsSqlContainer
        {
            get => _msSqlContainer ??
                   throw new InconclusiveException("Test fixture has not created a container for the MsSql server");
            private set => _msSqlContainer = value;
        }

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            MsSqlContainer = new TestcontainersBuilder<MsSqlTestcontainer>()
                .WithBizzkitDatabase(new MsSql2019TestcontainerDatabaseConfiguration(1451))
                .Build();

            await MsSqlContainer.StartAsync();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await MsSqlContainer.StopAsync();
            await MsSqlContainer.CleanUpAsync();
        }

    }
}