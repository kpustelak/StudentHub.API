using StudentHub.API.Models.Entities;

namespace StudentHub.API.Interface
{
    public interface ISemesterRepository
    {
        public Task<Semester?> GetSemesterByIdAsync(string semesterId);
        public Task<Semester> CreateSemesterAsync(Semester semester);
        public Task<Semester> UpdateSemesterAsync(Semester semester);
        public void DeleteSemester(Semester semester);
        public Task<Semester> AddUserToSemesterAsync(User user, Semester semester);
        public Task DeleteUserFromSemester(User user, Semester semester);
    }
}
