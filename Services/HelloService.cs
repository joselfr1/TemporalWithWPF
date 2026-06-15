using Model.Ports;

namespace Services;

public class HelloService: IHelloService
{
    public string SayHello(string name) => $"Hello, {name}!";
}
