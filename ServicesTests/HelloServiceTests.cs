using Model.Ports;
using Moq;
using Services;

namespace ServicesTests;

public class HelloServiceTests
{
    [Theory]
    [InlineData("", "es", "Hola")]
    [InlineData("jose", "en", "Hello")]
    [InlineData("Jose", "ru", "привет")]
    [InlineData("7845dsf4dsffdoiooi4$%%&%&%4{}lllÑ", "jp", "こんにちは")]
    public void Test_Names(string name,string languageCode, string template)
    {
        Mock<ILanguageRepository> repositoryMock = new();
        repositoryMock.Setup(repository => repository.GetGreetingMessage(languageCode)).Returns(template);
        var service = new HelloService(repositoryMock.Object);
        var result = service.SayHello(name, template);
        Assert.Equal($"{template}, {name}!",result);
    }

    [Theory]
    [InlineData("es", "Hola")]
    [InlineData("en", "Hello")]
    [InlineData("ru", "привет")]
    [InlineData("jp", "こんにちは")]
    public void Test_Languages(string languageCode, string template)
    {
        Mock<ILanguageRepository> repositoryMock = new();
        repositoryMock.Setup(repository => repository.GetGreetingMessage(languageCode)).Returns(template);
        var service = new HelloService(repositoryMock.Object);
        var result = service.GetGreetingMessage(languageCode);
        Assert.Equal(template, result);
    }

    [Fact]
    public void Test_Empty_Language()
    {
        string? nullableResult = null;
        Mock<ILanguageRepository> repositoryMock = new();
        repositoryMock.Setup(repository => repository.GetGreetingMessage(It.IsAny<string>())).Returns(nullableResult);
        var service = new HelloService(repositoryMock.Object);
        var result = service.GetGreetingMessage("test");
        Assert.Null(result);
    }
}
