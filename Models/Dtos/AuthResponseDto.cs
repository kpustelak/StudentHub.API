using StudentHub.API.Models.Entities;

namespace StudentHub.API.Models.Dtos
{
    public record AuthResponseDto(UserDto Dto, string Token);
}
