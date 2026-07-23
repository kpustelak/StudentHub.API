using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentHub.API.Interface;
using StudentHub.API.Models;
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
        public async Task<ActionResult<ResponseModel<UserDto>>> GetUserInfo()
        { 
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?.ToString();
            if(string.IsNullOrWhiteSpace(userId)) return Unauthorized("User ID not found in claims.");

            try
            {
                var userFromDb = await _userService.GetUserByIdAsync(userId);
                var us = new UserDto(
                    userId,
                    userFromDb.DiscordId,
                    userFromDb.Username,
                    userFromDb.AvatarUrl,
                    userFromDb.Semesters.Select(s => new SemesterDto(
                        s.Id,
                        s.Title,
                        s.ShortTitle,
                        s.Description,
                        s.StartDate,
                        s.EndDate
                    )).ToList(),
                    userFromDb.StudentGroups.Select(g => new StudentGroupDto(
                        g.Id,
                        g.Name,
                        g.Description
                    )).ToList());

                return Ok(new ResponseModel<UserDto>
                {
                    Data = us,
                    Status = true,
                    Message = "User data retrieved successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseModel<UserDto>
                {
                    Status = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<UserDto>
                {
                    Status = false,
                    Message = ex.Message
                });
            }
        }
    }
}
