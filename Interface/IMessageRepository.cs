using StudentHub.API.Models.Entities;

namespace StudentHub.API.Interface
{
    public interface IMessageRepository
    {
        Task AddMessageAsync(Message message);
        Task DeleteMessageAsync(Message message);
        Task<Message?> GetMessageAsync(string id);
        Task<List<Message>> GetMessagesWithPagingAsync(string groupId, int page, int pageSize);
    }
}
