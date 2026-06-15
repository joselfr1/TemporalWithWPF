using Model.Ports;
using Moq;
using Temporalio.Client;
using Temporalio.Testing;
using Temporalio.Worker;
using Workflow.Workflows;

namespace WorkflowTests
{
    public class SayHelloWorkflowTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("jose")]
        [InlineData("Jose")]
        [InlineData("7845dsf4dsffdoiooi4$%%&%&%4{}lllÑ")]
        public async Task WorkflowTests(string name)
        {
            var mockActivity = new Mock<IHelloActivity>();
            mockActivity
                .Setup(act => act.SayHello(name))
                .Returns($"Hello, {name}!");

            await using var env = await WorkflowEnvironment.StartTimeSkippingAsync();
            var workerOptions = new TemporalWorkerOptions($"test-queue-{Guid.NewGuid()}")
            .AddWorkflow<SayHelloWorkflow>()
            .AddActivity(mockActivity.Object.SayHello);

            using var worker = new TemporalWorker(env.Client, workerOptions);

            await worker.ExecuteAsync(async () =>
            {
                // Execute the workflow against our test environment
                var result = await env.Client.ExecuteWorkflowAsync(
                    (SayHelloWorkflow wf) => wf.RunAsync(name),
                    new(id: $"test-id-{Guid.NewGuid()}", taskQueue: worker.Options.TaskQueue!)
                );

                // Assert: Verify that the workflow returned exactly what the mock provided
                Assert.Equal($"Hello, {name}!", result);
            }, TestContext.Current.CancellationToken);

            mockActivity.Verify(act => act.SayHello(name), Times.Once);
        }
    }
}
