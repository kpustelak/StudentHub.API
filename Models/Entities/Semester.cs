namespace StudentHub.API.Models.Entities
{
    public class Semester
    {
        public required string Id { get; set; }
        public required string Title { get; set; }
        public required string ShortTitle { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public IList<StudentGroup> StudentGroups { get; set; } = [];
        public IList<User> Students { get; set; } = [];
    }
}
