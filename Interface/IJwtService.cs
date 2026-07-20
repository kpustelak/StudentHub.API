using StudentHub.API.Models.Entities;

namespace StudentHub.API.Interface
{
    public interface IJwtService
    {
        public string GenerateToken(User user);
    }
}
