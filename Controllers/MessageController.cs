using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _service;
        public MessageController(IMessageService service)
        {
            _service = service;
        }

        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<ResponseModel<List<MessageDto>>>> GetMessages(
        string groupId,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 50)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("User ID not found in claims.");
            try
            {
                var messages = await _service.GetMessagesAsync(groupId, userId, page, pageSize);
                return Ok(new ResponseModel<List<MessageDto>>
                {
                    Status = true,
                    Message = "Messages retrieved successfully.",
                    Data = messages
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseModel<List<MessageDto>> { Status = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseModel<List<MessageDto>> { Status = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<List<MessageDto>> { Status = false, Message = ex.Message });
            }
        }
    }
}
