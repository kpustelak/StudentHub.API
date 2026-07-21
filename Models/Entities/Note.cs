namespace StudentHub.API.Models.Entities
{
    public class Note
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsReported { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int ViewCount { get; set; }

        public IList<User> Contributors { get; set; } = [];
        public IList<StudentGroup> StudentGroups { get; set; } = [];
        public IList<File> Files { get; set; } = [];
    }
}
