using StudentHub.API.Interface;
using StudentHub.API.Models.Dtos;
using StudentHub.API.Models.Entities;

namespace StudentHub.API.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repository;
        private readonly IGroupAccessService _groupAccessService;

        public MessageService(
            IMessageRepository repository,
            IGroupAccessService groupAccessService)
        {
            _repository = repository;
            _groupAccessService = groupAccessService;
        }

        public async Task DeleteMessageAsync(string messageId, string userId)
        {
            var message = await _repository.GetMessageAsync(messageId);
            if (message == null) throw new ArgumentException("Cannot find message.");
            if (message.StudentId != userId) throw new UnauthorizedAccessException("User is not owner of this message.");
            await _repository.DeleteMessageAsync(message);
        }

        public async Task<List<MessageDto>> GetMessagesAsync(string groupId, string userId, int page, int pageSize)
        {
            await _groupAccessService.EnsureMemberAsync(groupId, userId);

            page = Math.Max(page, 0);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var messages = await _repository.GetMessagesWithPagingAsync(groupId, page, pageSize);
            return messages.Select(MapToDto).ToList();
        }

        public async Task<MessageDto> SendAsync(AddMessageDto dto, string userId)
        {
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                throw new ArgumentException("Message content cannot be empty.");
            }

            await _groupAccessService.EnsureMemberAsync(dto.StudentGroupId, userId);

            var message = new Message
            {
                Id = Guid.NewGuid().ToString(),
                Content = dto.Content.Trim(),
                CreatedAt = DateTime.UtcNow,
                StudentId = userId,
                StudentGroupId = dto.StudentGroupId
            };
            await _repository.AddMessageAsync(message);

            var saved = await _repository.GetMessageAsync(message.Id);
            return MapToDto(saved!);
        }

        private static MessageDto MapToDto(Message message)
        {
            return new MessageDto
            {
                Id = message.Id,
                UserId = message.StudentId,
                Content = message.Content,
                CreatedAt = message.CreatedAt,
                UserName = message.Student?.Username ?? string.Empty
            };
        }
    }
}
