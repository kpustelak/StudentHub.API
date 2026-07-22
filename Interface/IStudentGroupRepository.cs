using StudentHub.API.Models.Entities;

namespace StudentHub.API.Interface
{
    public interface IStudentGroupRepository
    {
        Task<StudentGroup?> GetByIdWithMembersAsync(string groupId);
    }
}
