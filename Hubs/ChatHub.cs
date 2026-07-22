using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StudentHub.API.Interface;
using StudentHub.API.Models.Dtos;
using System.Security.Claims;

namespace StudentHub.API.Hubs
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ChatHub : Hub
    {
        private readonly IGroupAccessService _access;
        private readonly IMessageService _messageService;
        public ChatHub(IGroupAccessService access, IMessageService messageService)
        {
            _access = access;
            _messageService = messageService;
        }
        public async Task JoinGroup(string studentGroupId)
        {
                var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userId))  throw new HubException("User doesnt have identifier.");
            try
            {
                await _access.EnsureMemberAsync(studentGroupId, userId);
                await Groups.AddToGroupAsync(Context.ConnectionId, studentGroupId);
            }
            catch (ArgumentException)
            {
                throw new HubException("Group not found.");
            }
            catch (InvalidOperationException)
            {
                throw new HubException("User doesn't have access to this group.");
            }

        }

        public async Task LeaveGroup(string studentGroupId)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) throw new HubException("User doesnt have identifier.");

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, studentGroupId);
        }

        public async Task SendMessage(string studentGroupId, string content)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) throw new HubException("User doesn't have identifier.");

            try
            {
                var dto = new AddMessageDto
                {
                    StudentGroupId = studentGroupId,
                    Content = content
                };

                var messageDto = await _messageService.SendAsync(dto, userId);
                await Clients.Group(studentGroupId).SendAsync("ReceiveMessage", messageDto);
            }
            catch (ArgumentException ex)
            {
                throw new HubException(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                throw new HubException(ex.Message);
            }
        }
    }
}