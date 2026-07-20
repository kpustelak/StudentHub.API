namespace StudentHub.API.Models.Dtos
{
    public record AddSemesterDto(string Title, string ShortTitle, string Description, DateOnly StartDate, DateOnly EndDate);
}
