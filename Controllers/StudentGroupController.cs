using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentHub.API.Interface;
using StudentHub.API.Models;
using StudentHub.API.Models.Dtos;
using System.Security.Claims;

namespace StudentHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class StudentGroupController : ControllerBase
    {
        private readonly IStudentGroupService _service;

        public StudentGroupController(IStudentGroupService service)
        {
            _service = service;
        }

        [HttpGet("my")]
        public async Task<ActionResult<ResponseModel<List<StudentGroupDto>>>> GetMyGroups()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("User ID not found in claims.");
            }

            try
            {
                var groups = await _service.GetUserStudentGroupsAsync(userId);
                return Ok(new ResponseModel<List<StudentGroupDto>>
                {
                    Status = true,
                    Message = "Student groups retrieved successfully.",
                    Data = groups
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseModel<List<StudentGroupDto>> { Status = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<List<StudentGroupDto>> { Status = false, Message = ex.Message });
            }
        }

        [HttpGet("semester/{semesterId}")]
        public async Task<ActionResult<ResponseModel<List<StudentGroupDto>>>> GetSemesterGroups(string semesterId)
        {
            try
            {
                var groups = await _service.GetSemesterGroupsAsync(semesterId);
                return Ok(new ResponseModel<List<StudentGroupDto>>
                {
                    Status = true,
                    Message = "Semester student groups retrieved successfully.",
                    Data = groups
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseModel<List<StudentGroupDto>> { Status = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<List<StudentGroupDto>> { Status = false, Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<StudentGroupDto>>> GetById(string id)
        {
            try
            {
                var group = await _service.GetStudentGroupByIdAsync(id);
                return Ok(new ResponseModel<StudentGroupDto>
                {
                    Status = true,
                    Message = "Student group retrieved successfully.",
                    Data = group
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseModel<StudentGroupDto> { Status = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<StudentGroupDto> { Status = false, Message = ex.Message });
            }
        }

        [HttpPost("semester/{semesterId}")]
        public async Task<ActionResult<ResponseModel<StudentGroupDto>>> Create(string semesterId, [FromBody] AddStudentGroupDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("User ID not found in claims.");
            }

            try
            {
                var group = await _service.AddStudentGroupAsync(dto, semesterId, userId);
                return Ok(new ResponseModel<StudentGroupDto>
                {
                    Status = true,
                    Message = "Student group created successfully.",
                    Data = group
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseModel<StudentGroupDto> { Status = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<StudentGroupDto> { Status = false, Message = ex.Message });
            }
        }

        [HttpPost("join/{groupId}")]
        public async Task<ActionResult<ResponseModel<StudentGroupDto>>> Join(string groupId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("User ID not found in claims.");
            }

            try
            {
                var group = await _service.AddUserToStudentGroupAsync(groupId, userId);
                return Ok(new ResponseModel<StudentGroupDto>
                {
                    Status = true,
                    Message = "User joined student group successfully.",
                    Data = group
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseModel<StudentGroupDto> { Status = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseModel<StudentGroupDto> { Status = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<StudentGroupDto> { Status = false, Message = ex.Message });
            }
        }

        [HttpPost("leave/{groupId}")]
        public async Task<ActionResult<ResponseModel<EmptyResult>>> Leave(string groupId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("User ID not found in claims.");
            }

            try
            {
                await _service.RemoveUserFromStudentGroupAsync(groupId, userId);
                return Ok(new ResponseModel<EmptyResult>
                {
                    Status = true,
                    Message = "User left student group successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseModel<EmptyResult> { Status = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseModel<EmptyResult> { Status = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<EmptyResult> { Status = false, Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel<EmptyResult>>> Delete(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("User ID not found in claims.");
            }

            try
            {
                await _service.DeleteStudentGroupAsync(id, userId);
                return Ok(new ResponseModel<EmptyResult>
                {
                    Status = true,
                    Message = "Student group deleted successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseModel<EmptyResult> { Status = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseModel<EmptyResult> { Status = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<EmptyResult> { Status = false, Message = ex.Message });
            }
        }
    }
}
