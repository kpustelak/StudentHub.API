namespace StudentHub.API.Models.Dtos
{
    public record NoteDto(
        string Id,
        string Name,
        string Description,
        List<FileDto> Files,
        bool IsActive,
        bool IsReported,
        int ViewCount,
        List<string> ContributorIds,
        List<string> StudentGroupIds
    );
}
