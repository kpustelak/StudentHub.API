using Microsoft.EntityFrameworkCore;
using StudentHub.API.Context;
using StudentHub.API.Interface;
using StudentHub.API.Models.Entities;

namespace StudentHub.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly StudentHubDbContext _db;
        public UserRepository(StudentHubDbContext db)
        {
            _db = db;
        }
        public async Task<User?> CreateUserAsync(User user)
        { 
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task DeleteUserAsync(User user)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }

        public async Task<User?> GetUserByDiscordIdAsync(string discordId)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.DiscordId == discordId);
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> UpdateUserAsync(User newUser)
        {
            _db.Users.Update(newUser);
            await _db.SaveChangesAsync();
            return newUser;
        }
    }
}
