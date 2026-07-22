using StudentHub.API.Models.Dtos;

namespace StudentHub.API.Interface
{
    public interface IMessageService
    {
        public Task<MessageDto> SendAsync(AddMessageDto dto, string userId);
        public Task<List<MessageDto>> GetMessagesAsync(string groupId, string userId ,int page, int pageSize);
        public Task DeleteMessageAsync(string messageId, string userId);
    }
}
