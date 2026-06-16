namespace Model.Ports;

public interface IHelloService
{
    string SayHello(string name, string languageTemplate);
    string? GetGreetingMessage(string name);
}
