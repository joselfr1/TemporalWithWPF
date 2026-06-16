using Model.Ports;
using Moq;
using Temporalio.Testing;
using Workflow.Activities;

namespace WorkflowTests;

public class GreetingLanguageActivityTests
{
    [Theory]
    [InlineData("es", "Hola")]
    [InlineData("en", "Hello")]
    [InlineData("ru", "привет")]
    [InlineData("jp", "こんにちは")]
    public async Task ActivityTests(string languageCode, string template)
    {
        var serviceMock = new Mock<IHelloService>();
        serviceMock.Setup(service => service.GetGreetingMessage(languageCode)).Returns(template);
        var activity = new GreetingLanguageActivity(serviceMock.Object);

        var env = new ActivityEnvironment();

        var result = await env.RunAsync(() => activity.GetGreetingMessage(languageCode));
        Assert.Equal(template, result);
    }
}
