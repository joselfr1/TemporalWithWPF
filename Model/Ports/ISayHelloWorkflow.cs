namespace Model.Ports;

using Model.Dtos;
using Temporalio.Workflows;

[Workflow]
public interface ISayHelloWorkflow
{
    [WorkflowRun]
    Task<string> RunAsync(HelloDto hello);
}
