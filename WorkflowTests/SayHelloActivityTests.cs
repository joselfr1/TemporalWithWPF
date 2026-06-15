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
    [InlineData("")]
    [InlineData("jose")]
    [InlineData("Jose")]
    [InlineData("7845dsf4dsffdoiooi4$%%&%&%4{}lllÑ")]
    public async Task ActivityTests(string name)
    {
        var serviceMock = new Mock<IHelloService>();
        serviceMock.Setup(service => service.SayHello(name)).Returns($"Hello, {name}!");
        var activity = new HelloActivity(serviceMock.Object);

        var env = new ActivityEnvironment();

        var result = await env.RunAsync(() => activity.SayHello(name));
        Assert.Equal($"Hello, {name}!", result);
    }
}
