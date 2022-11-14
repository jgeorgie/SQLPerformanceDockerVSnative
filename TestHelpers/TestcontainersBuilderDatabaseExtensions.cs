using DotNet.Testcontainers.Builders;
using TestContainersSQL.TestHelpers.Configuration;

namespace TestContainersSQL.TestHelpers
{
    public static class TestcontainersBuilderDatabaseExtensions
    {
        public static ITestcontainersBuilder<T> WithBizzkitDatabase<T>(this ITestcontainersBuilder<T> builder,
            TestcontainerDatabaseConfiguration configuration) where T : TestcontainerDatabase
        {
            builder = configuration.Environments.Aggregate(builder, (current, environment) =>
                current.WithEnvironment(environment.Key, environment.Value));

            return builder
                .WithImage(configuration.Image)
                .WithPortBinding(configuration.Port, configuration.DefaultPort)
                .WithOutputConsumer(configuration.OutputConsumer)
                .WithWaitStrategy(configuration.WaitStrategy)
                .ConfigureContainer(container =>
                {
                    container.ContainerPort = configuration.DefaultPort;
                    container.Username = configuration.Username;
                    container.Password = configuration.Password;
                });
        }
    }
}
