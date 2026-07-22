using Microsoft.EntityFrameworkCore;
using StudentHub.API.Context;
using StudentHub.API.Interface;
using StudentHub.API.Migrations;
using StudentHub.API.Models.Entities;

namespace StudentHub.API.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly StudentHubDbContext _db;
        public MessageRepository(StudentHubDbContext db)
        {
            _db = db;
        }
        public async Task AddMessageAsync(Models.Entities.Message message) {
            await _db.Messages.AddAsync(message);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteMessageAsync(Models.Entities.Message message)
        {
            _db.Messages.Remove(message);
            await _db.SaveChangesAsync();
        }

        public async Task<Models.Entities.Message?> GetMessageAsync(string id)
        {
            return await _db.Messages
                .Include(s => s.Student)
                .Include(g => g.StudentGroup)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Models.Entities.Message>> GetMessagesWithPagingAsync(string groupId, int page, int pageSize)
        {
            return await _db.Messages
                .Include(s => s.Student)
                .Include(g => g.StudentGroup)
                .Where(x => x.StudentGroupId == groupId)
                .OrderBy(x => x.CreatedAt)
                .Skip(pageSize * page)
                .Take(pageSize)
                .ToListAsync();  
        }
    }
}
