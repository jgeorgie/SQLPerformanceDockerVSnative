using DotNet.Testcontainers.Configurations;

namespace TestContainersSQL.TestHelpers.Configuration
{
    public abstract class TestcontainerDatabaseConfiguration : HostedServiceConfiguration
    {
        public TestcontainerDatabaseConfiguration(string image, int defaultPort, int port = 0) : base(image, defaultPort, port)
        {
        }
    }
}
