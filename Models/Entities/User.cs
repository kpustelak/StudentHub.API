namespace StudentHub.API.Models.Entities
{
    public class User
    {
        public required string Id { get; set; }
        public required string DiscordId { get; set; }
        public required string Username { get; set; }
        public string AvatarUrl { get; set; } = string.Empty;

        public IList<Semester> Semesters { get; set; } = [];
        public IList<StudentGroup> StudentGroups { get; set; } = [];
        public IList<Note> Notes { get; set; } = [];
        public IList<Message> Messages { get; set; } = [];
    }
}
