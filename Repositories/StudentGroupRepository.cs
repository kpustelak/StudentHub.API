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

        public Task<StudentGroup?> GetByIdAsync(string groupId)
        {
            return _db.StudentGroups
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == groupId);
        }

        public async Task CreateStudentGroupAsync(StudentGroup studentGroup)
        {
            await _db.StudentGroups.AddAsync(studentGroup);
            await _db.SaveChangesAsync();
        }

        public async Task EditStudentGroupAsync(StudentGroup studentGroup)
        {
            _db.StudentGroups.Update(studentGroup);
            await _db.SaveChangesAsync();
        }

        public async Task<List<StudentGroup>> GetAllAsync()
        {
            return await _db.StudentGroups.ToListAsync();
        }

        public async Task<List<StudentGroup>> GetSemesterGroupsAsync(string semesterId)
        {
            return await _db.StudentGroups
                .Where(x => x.SemesterId == semesterId)
                .ToListAsync();
        }

        public async Task RemoveStudentGroupAsync(StudentGroup group)
        {
            _db.StudentGroups.Remove(group);
            await _db.SaveChangesAsync();
        }

        public async Task<List<StudentGroup>> GetStudentGroupsAsync(string userId)
        {
            return await _db.StudentGroups
                .Where(x => x.Members.Any(m => m.Id == userId))
                .ToListAsync();
        }

        public async Task AddMemberAsync(StudentGroup group, User user)
        {
            group.Members.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task RemoveMemberAsync(StudentGroup group, string userId)
        {
            var member = group.Members.FirstOrDefault(m => m.Id == userId);
            if (member is not null)
            {
                group.Members.Remove(member);
                await _db.SaveChangesAsync();
            }
        }
    }
}
