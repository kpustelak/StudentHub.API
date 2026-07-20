namespace StudentHub.API.Models.Dtos
{
    public class EditUserDto
    {
        public required string Username { get; set; }
        public string AvatarUrl { get; set; } = string.Empty;
    }
}
