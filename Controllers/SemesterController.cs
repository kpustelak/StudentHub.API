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
    public class SemesterController : ControllerBase
    {
        private readonly ISemesterService _semesterService;
        public SemesterController(ISemesterService semesterService)
        {
            _semesterService = semesterService;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseModel<Semester>>> CreateSemester([FromBody] AddSemesterDto dto)
        {
            try
            {
                var semester = await _semesterService.CreateSemesterAsync(dto);
                return Ok(new ResponseModel<Semester>
                {
                    Status = true,
                    Message = "Semester created successfully.",
                    Data = semester
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseModel<Semester>
                {
                    Status = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<Semester>
                {
                    Status = false,
                    Message = ex.Message
                });
            }
        }
        
        [HttpGet]
        public async Task<ActionResult<ResponseModel<IEnumerable<SemesterDto>>>> ListSemesters()
        {
            try
            {
                var semesters = await _semesterService.GetSemestersAsync();
                var semesterDtos = await Task.FromResult(semesters.Select(s => new SemesterDto(s.Id, s.Title, s.ShortTitle, s.Description, s.StartDate, s.EndDate)));
                return Ok(new ResponseModel<IEnumerable<SemesterDto>>
                {
                    Status = true,
                    Message = "Semesters retrieved successfully.",
                    Data = semesterDtos
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<IEnumerable<SemesterDto>>
                {
                    Status = false,
                    Message = ex.Message
                });
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<Semester>>> GetSemesterById(string id)
        {
            try
            {
                var semester = await _semesterService.GetSemesterByIdAsync(id);
                return Ok(new ResponseModel<Semester>
                {
                    Status = true,
                    Message = "Semester retrieved successfully.",
                    Data = semester
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseModel<Semester>
                {
                    Status = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<Semester>
                {
                    Status = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("join/{semesterId}")]
        public async Task<ActionResult<ResponseModel<Semester>>> JoinSemester(string semesterId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?.ToString();

            try
            {
                ArgumentException.ThrowIfNullOrEmpty(semesterId);
                ArgumentException.ThrowIfNullOrEmpty(userId);
                var semester = await _semesterService.AddUserToSemester(userId, semesterId);
                return Ok(new ResponseModel<Semester>
                {
                    Status = true,
                    Message = "User joined semester successfully.",
                    Data = semester
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseModel<Semester>
                {
                    Status = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<Semester>
                {
                    Status = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("leave/{semesterId}")]
        public async Task<ActionResult<ResponseModel<EmptyResult>>> LeaveSemester(string semesterId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?.ToString();

            try
            {
                ArgumentException.ThrowIfNullOrEmpty(semesterId);
                ArgumentException.ThrowIfNullOrEmpty(userId);

                await _semesterService.DeleteUserFromSemester(userId, semesterId);
                return Ok(new ResponseModel<EmptyResult>
                {
                    Status = true,
                    Message = "User left semester successfully.",
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseModel<EmptyResult>
                {
                    Status = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<EmptyResult>
                {
                    Status = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel<Semester>>> UpdateSemester(string id, [FromBody] SemesterDto dto)
        {
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(id);
                var semester = await _semesterService.UpdateSemesterAsync(new Semester
                {
                    Id = id,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    ShortTitle = dto.ShortTitle,
                    Description = dto.Description,
                    Title = dto.Title
                }, id);
                return Ok(new ResponseModel<Semester>
                {
                    Status = true,
                    Message = "Semester updated successfully.",
                    Data = semester
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseModel<Semester>
                {
                    Status = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<Semester>
                {
                    Status = false,
                    Message = ex.Message
                });
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel<EmptyResult>>> DeleteSemester(string id)
        {
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(id);
                await _semesterService.DeleteSemesterAsync(id);
                return Ok(new ResponseModel<EmptyResult>
                {
                    Status = true,
                    Message = "Semester deleted successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseModel<EmptyResult>
                {
                    Status = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<EmptyResult>
                {
                    Status = false,
                    Message = ex.Message
                });
            }
        }
    }
}
