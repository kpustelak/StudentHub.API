using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentHub.API.Interface;
using StudentHub.API.Models.Dtos;
using StudentHub.API.Models.Entities;
using System.Security.Claims;

namespace StudentHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetUserInfo()
        { 
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?.ToString();
            if(string.IsNullOrWhiteSpace(userId)) return Unauthorized("User ID not found in claims.");

            try
            {
                var userData = await _userService.GetUserByIdAsync(userId);
                return Ok(new UserDto(userData.Id, userData.DiscordId, userData.Username, userData.AvatarUrl));
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
