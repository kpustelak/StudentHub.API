using StudentHub.API.Models.Dtos;
using StudentHub.API.Models.Entities;

namespace StudentHub.API.Interface
{
    public interface IUserService
    {
        public Task<User?> GetUserByDiscordIdAsync(string discordId);
        public Task<User> GetUserByIdAsync(string userId);
        public Task<User> CreateOrUpdateUserAsync(string discordId, string username, string discordAvatarUrl);
        public Task<User> EditUserAsync(EditUserDto dto, string id);
        public Task RemoveUserByIdAsync(string id);
    }
}
