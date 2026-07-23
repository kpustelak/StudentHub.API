using Microsoft.EntityFrameworkCore;
using StudentHub.API.Context;
using StudentHub.API.Interface;
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

        public async Task AddMessageAsync(Message message)
        {
            await _db.Messages.AddAsync(message);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteMessageAsync(Message message)
        {
            message.IsDeleted = true;
            await _db.SaveChangesAsync();
        }

        public async Task<Message?> GetMessageAsync(string id)
        {
            return await _db.Messages
                .Include(s => s.Student)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<List<Message>> GetMessagesWithPagingAsync(string groupId, int page, int pageSize)
        {
            return await _db.Messages
                .Include(s => s.Student)
                .Where(x => x.StudentGroupId == groupId && !x.IsDeleted)
                .OrderBy(x => x.CreatedAt)
                .Skip(pageSize * page)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
