using StudentHub.API.Models.Entities;

namespace StudentHub.API.Interface
{
    public interface INoteRepository
    {
        Task<Note?> GetNoteByIdAsync(string noteId);
        Task<List<Note>> GetNotesByGroupIdAsync(string groupId);
        Task<Note> CreateNoteAsync(Note note);
        Task<Note> UpdateNoteAsync(Note note);
    }
}
