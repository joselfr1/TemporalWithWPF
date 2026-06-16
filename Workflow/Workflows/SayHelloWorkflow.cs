namespace Workflow.Workflows;

using Model.Dtos;
using Model.Ports;
using Temporalio.Workflows;

[Workflow]
public class SayHelloWorkflow: ISayHelloWorkflow
{
    [WorkflowRun]
    public async Task<string> RunAsync(HelloDto hello)
    {
        var languageTemplate = await Workflow.ExecuteActivityAsync(
            (IGreetingLanguageActivity act) => act.GetGreetingMessage(hello.LanguageCode),
            new() { StartToCloseTimeout = TimeSpan.FromMinutes(5) });

        if (languageTemplate == null)
        {
            return $"Sorry {hello.Name}, i don't understand the language";
        }

        return await Workflow.ExecuteActivityAsync(
            (IHelloActivity act) => act.SayHello(hello.Name, languageTemplate),
            new() { StartToCloseTimeout = TimeSpan.FromMinutes(5) }
        );
    }
}