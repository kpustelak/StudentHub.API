namespace StudentHub.API.Models.Dtos
{
    public class AddMessageDto
    {
        public required string StudentGroupId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
