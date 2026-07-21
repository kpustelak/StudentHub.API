namespace StudentHub.API.Models.Dtos
{
    public record SemesterDto(
    string Id,
    string Title,
    string ShortTitle,
    string Description,
    DateOnly StartDate,
    DateOnly EndDate
    );
}
