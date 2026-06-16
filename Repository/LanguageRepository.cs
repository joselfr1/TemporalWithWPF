using Model.Ports;
using System.Collections.Concurrent;

namespace Repository;

public class LanguageRepository : ILanguageRepository
{
    readonly ConcurrentDictionary<string, string> languageGreetings = new(StringComparer.OrdinalIgnoreCase);
    public LanguageRepository()
    {
        languageGreetings.TryAdd("en", "Hello");
        languageGreetings.TryAdd("es", "Hola");
        languageGreetings.TryAdd("fr", "Bonjour");
        languageGreetings.TryAdd("jp", "こんにちは");
        languageGreetings.TryAdd("it", "Ciao");
        languageGreetings.TryAdd("ru", "привет");
    }
    public string? GetGreetingMessage(string code)
    {
        if (languageGreetings.TryGetValue(code, out string? value))
        {
            return value;
        }

        return null;
    }
}
