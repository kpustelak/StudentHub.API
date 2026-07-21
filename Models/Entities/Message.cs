namespace StudentHub.API.Models.Entities
{
    public class Message
    {
        public required string Id { get; set; } 
        public required string StudentGroupId { get; set; }
        public required string StudentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public StudentGroup? StudentGroup { get; set; }
        public User? Student { get; set; }
    }
}
