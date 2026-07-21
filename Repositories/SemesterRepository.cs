using Microsoft.EntityFrameworkCore;
using StudentHub.API.Context;
using StudentHub.API.Interface;
using StudentHub.API.Models.Entities;

namespace StudentHub.API.Repositories
{
    public class SemesterRepository : ISemesterRepository
    {
        private readonly StudentHubDbContext _db;
        public SemesterRepository(StudentHubDbContext db)
        { 
            _db = db;
        }

        public async Task<Semester> AddUserToSemesterAsync(User user, Semester semester)
        {
            semester.Students.Add(user);
            user.Semesters.Add(semester);
            _db.Semesters.Update(semester);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return semester;
        }
        public async Task<Semester> CreateSemesterAsync(Semester semester)
        {
            await _db.Semesters.AddAsync(semester);
            await _db.SaveChangesAsync();
            return semester;
        }
        public async Task DeleteSemesterAsync(Semester semester)
        {
            _db.Semesters.Remove(semester);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteUserFromSemester(User user, Semester semester)
        {
            semester.Students.Remove(user);
            user.Semesters.Remove(semester);
            _db.Semesters.Update(semester);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }
        public Task<Semester?> GetSemesterByIdAsync(string semesterId) {
            return _db.Semesters.Include(x => x.Students).FirstOrDefaultAsync(x => x.Id == semesterId);
        }

        public async Task<List<Semester>> GetSemestersAsync()
        {
            return await _db.Semesters.ToListAsync();
        }

        public async Task<Semester> UpdateSemesterAsync(Semester semester)
        {
            _db.Semesters.Update(semester);
            await _db.SaveChangesAsync();
            return semester;
        }
    }
}
