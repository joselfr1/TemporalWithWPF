using Model.Ports;

namespace Services;

public class HelloService(ILanguageRepository languageRepository) : IHelloService
{
    public string? GetGreetingMessage(string code)
    {
        return languageRepository.GetGreetingMessage(code);
    }

    public string SayHello(string name, string languageTemplate) => $"{languageTemplate}, {name}!";
}
