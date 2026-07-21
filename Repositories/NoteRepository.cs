using Microsoft.EntityFrameworkCore;
using StudentHub.API.Context;
using StudentHub.API.Interface;
using StudentHub.API.Models.Entities;

namespace StudentHub.API.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly StudentHubDbContext _db;

        public NoteRepository(StudentHubDbContext db)
        {
            _db = db;
        }

        public async Task<Note> CreateNoteAsync(Note note)
        {
            await _db.Notes.AddAsync(note);
            await _db.SaveChangesAsync();
            return (await GetNoteByIdAsync(note.Id)) ?? note;
        }

        public async Task<Note?> GetNoteByIdAsync(string noteId)
        {
            return await _db.Notes
                .Include(n => n.Contributors)
                .Include(n => n.StudentGroups)
                .Include(n => n.Files)
                .FirstOrDefaultAsync(n => n.Id == noteId && !n.IsDeleted);
        }

        public async Task<List<Note>> GetNotesByGroupIdAsync(string groupId)
        {
            return await _db.Notes
                .Include(n => n.Contributors)
                .Include(n => n.StudentGroups)
                .Include(n => n.Files)
                .Where(n => !n.IsDeleted && n.StudentGroups.Any(g => g.Id == groupId))
                .ToListAsync();
        }

        public Task<StudentGroup?> GetStudentGroupByIdAsync(string groupId)
        {
            return _db.StudentGroups
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == groupId);
        }

        public async Task<Note> UpdateNoteAsync(Note note)
        {
            _db.Notes.Update(note);
            await _db.SaveChangesAsync();
            return note;
        }
    }
}
