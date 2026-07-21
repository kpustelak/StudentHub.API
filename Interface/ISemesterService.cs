using StudentHub.API.Models.Dtos;
using StudentHub.API.Models.Entities;

namespace StudentHub.API.Interface
{
    public interface ISemesterService
    {
        public Task<Semester> GetSemesterByIdAsync(string id);
        public Task<List<Semester>> GetSemestersAsync();
        public Task<Semester> CreateSemesterAsync(AddSemesterDto dto);
        public Task<Semester> UpdateSemesterAsync(Semester dto, string id);
        public Task DeleteSemesterAsync(string id);
        public Task<Semester> AddUserToSemester(string userId, string semesterId);
        public Task DeleteUserFromSemester(string userId, string semesterId);
    }
}
