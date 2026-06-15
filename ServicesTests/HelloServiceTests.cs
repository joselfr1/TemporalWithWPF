using Services;

namespace ServicesTests;

public class HelloServiceTests
{
    [Theory]
    [InlineData("")]
    [InlineData("jose")]
    [InlineData("Jose")]
    [InlineData("7845dsf4dsffdoiooi4$%%&%&%4{}lllÑ")]
    public void TestNames(string name)
    {
        var service = new HelloService();
        var result = service.SayHello(name);
        Assert.Equal($"Hello, {name}!",result);
    }
}
