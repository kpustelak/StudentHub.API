using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentHub.API.Helpers;
using StudentHub.API.Interface;
using StudentHub.API.Models;
using StudentHub.API.Models.Dtos;
using StudentHub.API.Models.Entities;
using System.ComponentModel;
using System.Text;

namespace StudentHub.API.Controllers 
{ 

    [Route("api/[controller]")]
    [ApiController]
    public class DiscordLoginController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;
        private readonly LoginHelper _loginHelper;  
        public DiscordLoginController(IJwtService jwtService, IUserService userService, LoginHelper loginHelper)
        {
            _jwtService = jwtService;
            _userService = userService;
            _loginHelper = loginHelper;
        }

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
        [Description("Callback endpoint for Discord login. " +
        "Creating or updating a user based on Discord information.")]
        public async Task<ActionResult<ResponseModel<AuthResponseDto>>> Callback()
        {
            var result = await HttpContext.AuthenticateAsync("External");
            if (!result.Succeeded || result is null) return BadRequest("External authentication failed.");

            (string discordUserId, string discordUsername, string discordAvatarUrl)
                = _loginHelper.UnpackDiscordUserInfo(result);

            var user = await _userService.CreateOrUpdateUserAsync(discordUserId, discordUsername, discordAvatarUrl);

            var token = _jwtService.GenerateToken(user);
            await HttpContext.SignOutAsync("External");
            return Ok(new ResponseModel<AuthResponseDto>
            {
                Data = new AuthResponseDto(new UserDto(user.Id, user.DiscordId, user.Username, user.AvatarUrl), token),
                Message = "User authenticated successfully.",
                Status = true
            });
        }
    }
}
