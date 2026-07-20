using Microsoft.AspNetCore.Authentication;

namespace StudentHub.API.Helpers
{
    public class LoginHelper
    {
        public (string, string, string) UnpackDiscordUserInfo(AuthenticateResult result)
        {
            string? discordUserId = result.Principal?.FindFirst("urn:discord:id")?.Value;
            string? discordUsername = result.Principal?.FindFirst("urn:discord:username")?.Value;
            string? discordAvatarUrl = result.Principal?.FindFirst("urn:discord:avatar:url")?.Value ?? string.Empty;
            if (string.IsNullOrEmpty(discordUserId) ||
                string.IsNullOrEmpty(discordUsername))
                throw new Exception("Failed to retrieve Discord user information.");

            return (discordUserId, discordUsername, discordAvatarUrl);
        }
    }
}
