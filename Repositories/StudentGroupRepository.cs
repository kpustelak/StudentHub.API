using Microsoft.EntityFrameworkCore;
using StudentHub.API.Context;
using StudentHub.API.Interface;
using StudentHub.API.Models.Entities;

namespace StudentHub.API.Repositories
{
    public class StudentGroupRepository : IStudentGroupRepository
    {
        private readonly StudentHubDbContext _db;
        public StudentGroupRepository(StudentHubDbContext db)
        {
            _db = db;
        }
        public Task<StudentGroup?> GetByIdWithMembersAsync(string groupId)
        {
            return _db.StudentGroups
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == groupId);
        }
    }
}
