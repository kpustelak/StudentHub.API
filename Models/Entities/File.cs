namespace StudentHub.API.Models.Entities
{
    public class File
    {
        public required string Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public float Size { get; set; }
        public required string NoteId { get; set; }
        public Note? Note { get; set; }
    }
}
