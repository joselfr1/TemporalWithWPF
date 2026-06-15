using Microsoft.Extensions.DependencyInjection;
using Utils;

namespace WorkerTests
{
    public class ConfigurationBuilderTests
    {
        [Fact]
        public void CreateOptions_ShouldRegisterDependenciesAndReturnValidOptions()
        {
            // Arrange
            var services = new ServiceCollection();
            var queueName = "my-test-queue";

            // Act
            var options = services.CreateOptions(queueName);

            // Assert
            Assert.NotNull(options);
            Assert.Equal(queueName, options.TaskQueue);

            // Verify your workflow was registered
            Assert.Contains(options.Workflows, w => w.Name == "SayHelloWorkflow");
        }
    }
}
