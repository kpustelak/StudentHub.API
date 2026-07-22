using StudentHub.API.Interface;

namespace StudentHub.API.Services
{
    public class GroupAccessService : IGroupAccessService
    {
        private readonly IStudentGroupRepository _groups;
        public GroupAccessService(IStudentGroupRepository groups)
        {
            _groups = groups;
        }
        public async Task EnsureMemberAsync(string groupId, string userId)
        {
            var group = await _groups.GetByIdWithMembersAsync(groupId)
                ?? throw new ArgumentException("Student group not found.");
            if (group.Members.All(m => m.Id != userId))
            {
                throw new InvalidOperationException("User is not a member of this group.");
            }
        }
    }
}
