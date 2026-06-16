using Model.Ports;
using Temporalio.Activities;

namespace Workflow.Activities;

public class GreetingLanguageActivity(IHelloService helloService) : IGreetingLanguageActivity
{
    [Activity("GreetingLanguageActivity")]
    public string? GetGreetingMessage(string code)
    {
        return helloService.GetGreetingMessage(code);
    }
}
