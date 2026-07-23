namespace StudentHub.API.Models.Dtos
{
    public record UserDto(
        string Id,
        string DiscordId,
        string Username,
        string AvatarUrl,
        List<SemesterDto>? SemestersDto,
        List<StudentGroupDto>? StudentGroupsDto = null);
}
