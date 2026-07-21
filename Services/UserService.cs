using StudentHub.API.Interface;
using StudentHub.API.Models.Dtos;
using StudentHub.API.Models.Entities;

namespace StudentHub.API.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<User> CreateOrUpdateUserAsync(string discordId, string username, string discordAvatarUrl)
        {
            var userToFind = await _repository.GetUserByDiscordIdAsync(discordId);
            if (userToFind != null)
            {
                userToFind.Username = username;
                userToFind.AvatarUrl = discordAvatarUrl;
                var updatedUser = await _repository.UpdateUserAsync(userToFind);
                if (updatedUser == null) throw new ArgumentException("Cannot find user by discord ID");
                return updatedUser;
            }

            var user = await _repository.CreateUserAsync(new User
            {
                Id = Guid.NewGuid().ToString(),
                DiscordId = discordId,
                Username = username,
                AvatarUrl = discordAvatarUrl
            });

            var newUser = await _repository.GetUserByDiscordIdAsync(discordId);
            if (newUser == null) throw new ArgumentException("Cannot find user by discord ID");
            return newUser;
        }

        public Task<User?> GetUserByDiscordIdAsync(string discordId) => _repository.GetUserByDiscordIdAsync(discordId);

        public async Task<User> GetUserByIdAsync(string userId)
        {
            var userToFind = await _repository.GetUserByIdAsync(userId);
            if (userToFind == null) throw new ArgumentException("Cannot find user by ID");
            return userToFind;
        }

        public async Task<User> EditUserAsync(EditUserDto dto, string id)
        {
            var userToEdit = await _repository.GetUserByIdAsync(id);
            if (userToEdit == null) throw new ArgumentException("Cannot find user");

            userToEdit.Username = dto.Username;
            userToEdit.AvatarUrl = dto.AvatarUrl;

            var userToFind = await _repository.UpdateUserAsync(userToEdit);
            if (userToFind == null) throw new ArgumentException("Cannot find user by ID");
            return userToFind;
        }

        public async Task RemoveUserByIdAsync(string id)
        {
            var user = await _repository.GetUserByIdAsync(id);
            if (user == null) throw new ArgumentException("Cannot find user by ID");
            await _repository.DeleteUserAsync(user);
        }
    }
}
