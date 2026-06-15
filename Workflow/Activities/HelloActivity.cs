namespace Workflow.Activities;

using Model.Ports;
using Temporalio.Activities;

public class HelloActivity(IHelloService helloService) : IHelloActivity
{
    [Activity("SayHelloActivity")]
    public string SayHello(string name) => helloService.SayHello(name);

}
