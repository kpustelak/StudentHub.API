using StudentHub.API.Migrations;

namespace StudentHub.API.Interface
{
    public interface IMessageRepository
    {
        public Task AddMessageAsync(Models.Entities.Message message);
        public Task DeleteMessageAsync(Models.Entities.Message message);
        public Task<Models.Entities.Message?> GetMessageAsync(string id);
        public Task<List<Models.Entities.Message>> GetMessagesWithPagingAsync(string groupId,int page, int pageSize);

    }
}
