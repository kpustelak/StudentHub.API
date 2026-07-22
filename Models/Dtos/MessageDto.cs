namespace StudentHub.API.Models.Dtos
{
    public class MessageDto
    {
        public required string Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
