using Microsoft.AspNetCore.Http;

namespace StudentHub.API.Models.Dtos
{
    public class AddNoteDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string StudentGroupId { get; set; } = string.Empty;
        public IFormFile? File { get; set; }
    }
}
