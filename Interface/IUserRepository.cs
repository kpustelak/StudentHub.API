using StudentHub.API.Models.Entities;

namespace StudentHub.API.Interface
{
    public interface IUserRepository
    {
        public Task<User?> GetUserByDiscordIdAsync(string discordId);
        public Task<User?> GetUserByIdAsync(string userId);
        public Task<User?> CreateUserAsync(User user);
        public Task<User?> UpdateUserAsync(User user);
        public Task DeleteUserAsync(User user);
    }
}
