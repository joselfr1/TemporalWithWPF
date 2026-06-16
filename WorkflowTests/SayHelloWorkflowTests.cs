using Model.Dtos;
using Model.Ports;
using Moq;
using Temporalio.Client;
using Temporalio.Testing;
using Temporalio.Worker;
using Workflow.Workflows;

namespace WorkflowTests;

public class SayHelloWorkflowTests
{
    [Theory]
    [InlineData("","es", "Hola")]
    [InlineData("jose","en","Hello")]
    [InlineData("Jose","es","Hola")]
    [InlineData("7845dsf4dsffdoiooi4$%%&%&%4{}lllÑ","fr","Bonjour")]
    [InlineData("7845dsf4dsffdoiooi4$%%&%&%4{}lllÑ", "jp", "こんにちは")]
    public async Task WorkflowTests(string name,string languageCode,string languageMessage)
    {
        var mockHelloActivity = new Mock<IHelloActivity>();
        mockHelloActivity
            .Setup(act => act.SayHello(name, languageMessage))
            .Returns($"{languageMessage}, {name}!");

        var mockGreetingMessageActivity = new Mock<IGreetingLanguageActivity>();
        mockGreetingMessageActivity
            .Setup(act => act.GetGreetingMessage(languageCode))
            .Returns(languageMessage);

        await using var env = await WorkflowEnvironment.StartTimeSkippingAsync();
        var workerOptions = new TemporalWorkerOptions($"test-queue-{Guid.NewGuid()}")
        .AddWorkflow<SayHelloWorkflow>()
        .AddActivity(mockHelloActivity.Object.SayHello)
        .AddActivity(mockGreetingMessageActivity.Object.GetGreetingMessage);

        using var worker = new TemporalWorker(env.Client, workerOptions);
        var hello = new HelloDto() { LanguageCode = languageCode, Name = name };

        await worker.ExecuteAsync(async () =>
        {
            // Execute the workflow against our test environment
            var result = await env.Client.ExecuteWorkflowAsync(
                (SayHelloWorkflow wf) => wf.RunAsync(hello),
                new(id: $"test-id-{Guid.NewGuid()}", taskQueue: worker.Options.TaskQueue!)
            );

            // Assert: Verify that the workflow returned exactly what the mock provided
            Assert.Equal($"{languageMessage}, {name}!", result);
        }, TestContext.Current.CancellationToken);

        mockHelloActivity.Verify(act => act.SayHello(name,languageMessage), Times.Once);
    }
}
