namespace StudentHub.API.Interface
{
    public interface IGroupAccessService
    {
        Task EnsureMemberAsync(string groupId, string userId);
    }
}
