using StudentHub.API.Models.Dtos;
using StudentHub.API.Models.Entities;

namespace StudentHub.API.Interface
{
    public interface IMessageService
    {
        public Task<Message> SendAsync(AddMessageDto dto, string userId);
        public Task<List<MessageDto>> GetMessagesAsync(string groupId, string userId ,int page, int pageSize);
        public Task DeleteMessageAsync(string messageId, string userId);
    }
}
