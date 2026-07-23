using StudentHub.API.Models.Dtos;

namespace StudentHub.API.Interface
{
    public interface IStudentGroupService
    {
        Task<StudentGroupDto> AddStudentGroupAsync(AddStudentGroupDto dto, string semesterId, string userId);
        Task<StudentGroupDto> GetStudentGroupByIdAsync(string groupId);
        Task<List<StudentGroupDto>> GetUserStudentGroupsAsync(string userId);
        Task<List<StudentGroupDto>> GetSemesterGroupsAsync(string semesterId);
        Task DeleteStudentGroupAsync(string studentGroupId, string userId);
        Task<StudentGroupDto> AddUserToStudentGroupAsync(string studentGroupId, string userId);
        Task RemoveUserFromStudentGroupAsync(string studentGroupId, string userId);
    }
}
