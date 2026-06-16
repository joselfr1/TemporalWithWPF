using Model.Ports;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Temporalio.Testing;
using Temporalio.Worker;
using Workflow.Activities;
using Workflow.Workflows;

namespace WorkflowTests;

public class SayHelloActivityTests
{
    [Theory]
    [InlineData("","es","Hola")]
    [InlineData("jose","en","Hello")]
    [InlineData("Jose","ru", "привет")]
    [InlineData("7845dsf4dsffdoiooi4$%%&%&%4{}lllÑ", "jp", "こんにちは")]
    public async Task ActivityTests(string name,string languageCode, string template)
    {
        var serviceMock = new Mock<IHelloService>();
        serviceMock.Setup(service => service.SayHello(name,languageCode)).Returns($"{template}, {name}!");
        var activity = new HelloActivity(serviceMock.Object);

        var env = new ActivityEnvironment();

        var result = await env.RunAsync(() => activity.SayHello(name, languageCode));
        Assert.Equal($"{template}, {name}!", result);
    }
}
