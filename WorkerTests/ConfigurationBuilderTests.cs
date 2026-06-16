using Microsoft.Extensions.DependencyInjection;
using Utils;
using Workflow.Activities;

namespace UtilsTests;

public class ConfigurationBuilderTests
{
    [Fact]
    public void CreateOptions_ShouldRegisterDependenciesAndReturnValidOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var queueName = "my-test-queue";
        using var serviceProvider = services.BuildServiceProvider();
        // Act
        var options = services.CreateOptions(queueName,serviceProvider);

        // Assert
        Assert.NotNull(options);
        Assert.Equal(queueName, options.TaskQueue);

        // Verify your workflow was registered
        Assert.Contains(options.Workflows, w => w.Name == "SayHelloWorkflow");
    }

    [Fact]
    public void RegisterInfrastructure_ShouldRegisterDependenciesAndReturnValidOptions()
    {
        var services = new ServiceCollection();
        services.RegisterInfrastructure();
        using var rootProvider = services.BuildServiceProvider();

        // Act & Assert
        // Simulate Temporal opening a task execution scope
        using var scope = rootProvider.CreateScope();

        var scopedProvider = scope.ServiceProvider;

        // Verify concrete types resolve perfectly within an execution window
        Assert.NotNull(scopedProvider.GetService<HelloActivity>());
        Assert.NotNull(scopedProvider.GetService<GreetingLanguageActivity>());
    }
}
