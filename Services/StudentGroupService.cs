using StudentHub.API.Interface;
using StudentHub.API.Models.Dtos;
using StudentHub.API.Models.Entities;

namespace StudentHub.API.Services
{
    public class StudentGroupService : IStudentGroupService
    {
        private readonly IStudentGroupRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly ISemesterRepository _semesterRepository;

        public StudentGroupService(
            IStudentGroupRepository repository,
            IUserRepository userRepository,
            ISemesterRepository semesterRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
            _semesterRepository = semesterRepository;
        }

        public async Task<StudentGroupDto> AddStudentGroupAsync(AddStudentGroupDto dto, string semesterId, string userId)
        {
            _ = await _semesterRepository.GetSemesterByIdAsync(semesterId)
                ?? throw new ArgumentException("Semester not found.");

            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentException("User was not found.");

            var studentGroup = new StudentGroup
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Description = dto.Description,
                SemesterId = semesterId
            };
            studentGroup.Members.Add(user);

            await _repository.CreateStudentGroupAsync(studentGroup);

            var created = await _repository.GetByIdAsync(studentGroup.Id)
                ?? throw new InvalidOperationException("Student group was not created properly.");

            return MapToDto(created);
        }

        public async Task<StudentGroupDto> GetStudentGroupByIdAsync(string groupId)
        {
            var group = await _repository.GetByIdAsync(groupId)
                ?? throw new ArgumentException("Student group was not found.");

            return MapToDto(group);
        }

        public async Task<List<StudentGroupDto>> GetSemesterGroupsAsync(string semesterId)
        {
            _ = await _semesterRepository.GetSemesterByIdAsync(semesterId)
                ?? throw new ArgumentException("Semester not found.");

            var groups = await _repository.GetSemesterGroupsAsync(semesterId);
            return groups.Select(MapToDto).ToList();
        }

        public async Task<StudentGroupDto> AddUserToStudentGroupAsync(string studentGroupId, string userId)
        {
            var studentGroup = await _repository.GetByIdAsync(studentGroupId)
                ?? throw new ArgumentException("Student group was not found.");

            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentException("User was not found.");

            if (studentGroup.Members.Any(m => m.Id == userId))
            {
                throw new InvalidOperationException("User is already a member of this group.");
            }

            await _repository.AddMemberAsync(studentGroup, user);

            return MapToDto(studentGroup);
        }

        public async Task DeleteStudentGroupAsync(string studentGroupId, string userId)
        {
            var studentGroup = await _repository.GetByIdAsync(studentGroupId)
                ?? throw new ArgumentException("Student group was not found.");

            if (studentGroup.Members.All(m => m.Id != userId))
            {
                throw new InvalidOperationException("User is not a member of this group.");
            }

            await _repository.RemoveStudentGroupAsync(studentGroup);
        }

        public async Task<List<StudentGroupDto>> GetUserStudentGroupsAsync(string userId)
        {
            _ = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentException("User was not found.");

            var groups = await _repository.GetStudentGroupsAsync(userId);
            return groups.Select(MapToDto).ToList();
        }

        public async Task RemoveUserFromStudentGroupAsync(string studentGroupId, string userId)
        {
            var studentGroup = await _repository.GetByIdAsync(studentGroupId)
                ?? throw new ArgumentException("Student group was not found.");

            if (studentGroup.Members.All(m => m.Id != userId))
            {
                throw new InvalidOperationException("User is not a member of this group.");
            }

            await _repository.RemoveMemberAsync(studentGroup, userId);
        }

        private static StudentGroupDto MapToDto(StudentGroup group)
        {
            return new StudentGroupDto(group.Id, group.Name, group.Description);
        }
    }
}
