namespace Model.Ports;

public interface ILanguageRepository
{
    string? GetGreetingMessage(string code);
}
