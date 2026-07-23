using StudentHub.API.Models.Entities;

namespace StudentHub.API.Interface
{
    public interface IStudentGroupRepository
    {
        Task<StudentGroup?> GetByIdAsync(string groupId);
        Task CreateStudentGroupAsync(StudentGroup studentGroup);
        Task EditStudentGroupAsync(StudentGroup studentGroup);
        Task<List<StudentGroup>> GetAllAsync();
        Task<List<StudentGroup>> GetStudentGroupsAsync(string userId);
        Task<List<StudentGroup>> GetSemesterGroupsAsync(string semesterId);
        Task RemoveStudentGroupAsync(StudentGroup group);
        Task AddMemberAsync(StudentGroup group, User user);
        Task RemoveMemberAsync(StudentGroup group, string userId);
    }
}
