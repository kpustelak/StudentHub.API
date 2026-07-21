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
    public class NoteController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NoteController(INoteService noteService)
        {
            _noteService = noteService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ResponseModel<NoteDto>>> CreateNote([FromForm] AddNoteDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("User ID not found in claims.");
            }

            try
            {
                var note = await _noteService.CreateNoteAsync(dto, userId);
                return Ok(new ResponseModel<NoteDto>
                {
                    Status = true,
                    Message = "Note created successfully.",
                    Data = note
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseModel<NoteDto> { Status = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseModel<NoteDto> { Status = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<NoteDto> { Status = false, Message = ex.Message });
            }
        }

        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<ResponseModel<List<NoteDto>>>> GetNotesByGroup(string groupId)
        {
            try
            {
                var notes = await _noteService.GetNotesByGroupIdAsync(groupId);
                return Ok(new ResponseModel<List<NoteDto>>
                {
                    Status = true,
                    Message = "Notes retrieved successfully.",
                    Data = notes
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<List<NoteDto>> { Status = false, Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<NoteDto>>> GetNoteById(string id)
        {
            try
            {
                var note = await _noteService.GetNoteByIdAsync(id);
                return Ok(new ResponseModel<NoteDto>
                {
                    Status = true,
                    Message = "Note retrieved successfully.",
                    Data = note
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseModel<NoteDto> { Status = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<NoteDto> { Status = false, Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ResponseModel<NoteDto>>> UpdateNote(string id, [FromForm] EditNoteDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("User ID not found in claims.");
            }

            try
            {
                var note = await _noteService.UpdateNoteAsync(id, dto, userId);
                return Ok(new ResponseModel<NoteDto>
                {
                    Status = true,
                    Message = "Note updated successfully.",
                    Data = note
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseModel<NoteDto> { Status = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseModel<NoteDto> { Status = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<NoteDto> { Status = false, Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel<EmptyResult>>> DeleteNote(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("User ID not found in claims.");
            }

            try
            {
                await _noteService.DeleteNoteAsync(id, userId);
                return Ok(new ResponseModel<EmptyResult>
                {
                    Status = true,
                    Message = "Note deleted successfully."
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

        [HttpPost("{id}/report")]
        public async Task<ActionResult<ResponseModel<EmptyResult>>> ReportNote(string id)
        {
            try
            {
                await _noteService.ReportNoteAsync(id);
                return Ok(new ResponseModel<EmptyResult>
                {
                    Status = true,
                    Message = "Note reported successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseModel<EmptyResult> { Status = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<EmptyResult> { Status = false, Message = ex.Message });
            }
        }
    }
}
