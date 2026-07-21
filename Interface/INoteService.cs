using StudentHub.API.Models.Dtos;

namespace StudentHub.API.Interface
{
    public interface INoteService
    {
        Task<NoteDto> CreateNoteAsync(AddNoteDto dto, string userId);
        Task<NoteDto> GetNoteByIdAsync(string id);
        Task<List<NoteDto>> GetNotesByGroupIdAsync(string groupId);
        Task<NoteDto> UpdateNoteAsync(string id, EditNoteDto dto, string userId);
        Task DeleteNoteAsync(string id, string userId);
        Task ReportNoteAsync(string id);
    }
}
