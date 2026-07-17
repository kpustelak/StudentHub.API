using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StudentHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscordLoginController : ControllerBase
    {
        [HttpGet("login")]
        public IActionResult Login()
        {
            var properties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
            {
                RedirectUri = "/api/discordlogin/callback"
            };
            return Challenge(properties, "Discord");
        }


        [HttpGet("callback")]
        public async Task<IActionResult> Callback()
        {
            var result = await HttpContext.AuthenticateAsync("External");
            if (!result.Succeeded)
            {
                return BadRequest("External authentication failed.");
            }
            var discordUserId = result.Principal.FindFirst("urn:discord:id")?.Value;
            var discordUsername = result.Principal.FindFirst("urn:discord:username")?.Value;
            var discordAvatarUrl = result.Principal.FindFirst("urn:discord:avatar:url")?.Value;
            return Ok(new
            {
                Id = discordUserId,
                Username = discordUsername,
                AvatarUrl = discordAvatarUrl
            });
        }

    }
}
