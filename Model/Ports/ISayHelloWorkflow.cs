namespace Model.Ports;

using Temporalio.Workflows;

[Workflow]
public interface ISayHelloWorkflow
{
    [WorkflowRun]
    Task<string> RunAsync(string name);
}
