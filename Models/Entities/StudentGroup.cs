namespace StudentHub.API.Models.Entities
{
    public class StudentGroup
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;

        public required string SemesterId { get; set; }
        public Semester? Semester { get; set; }

        public IList<User> Members { get; set; } = [];
        public IList<Note> Notes { get; set; } = [];
    }
}
