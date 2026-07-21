using Microsoft.AspNetCore.Http;

namespace StudentHub.API.Models.Dtos
{
    public class EditNoteDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public IFormFile? File { get; set; }
    }
}
