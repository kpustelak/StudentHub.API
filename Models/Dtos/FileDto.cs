namespace StudentHub.API.Models.Dtos
{
    public record FileDto(
        string Id,
        string Title,
        string Url,
        float Size
    );
}
