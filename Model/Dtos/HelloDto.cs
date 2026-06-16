namespace Model.Dtos;

public record HelloDto
{
    public required string Name { get; set; }
    public required string LanguageCode { get; set; }
}
