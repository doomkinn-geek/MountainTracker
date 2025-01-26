using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MountainTracker.Core.DTO.User;
using MountainTracker.Core.Services;

namespace MountainTracker.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // если хотим, чтобы методы были доступны только авторизованным
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            // Получаем Id юзера из JWT claim'а, например ClaimTypes.NameIdentifier
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var userDto = await _userService.GetUserByIdAsync(userId.Value);
            if (userDto == null)
                return NotFound();

            return Ok(userDto);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            await _userService.UpdateUserProfileAsync(userId.Value, dto);
            return NoContent();
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            await _userService.ChangePasswordAsync(userId.Value, request.OldPassword, request.NewPassword);
            return NoContent();
        }

        // Пример дополнительного запроса или удаления
        // [HttpDelete("me")]
        // public async Task<IActionResult> DeleteMe() { ... }

        // ------------------------------------------
        // Helper method to get userId from JWT claims
        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return null;
            if (Guid.TryParse(userIdClaim.Value, out var guidVal))
                return guidVal;
            return null;
        }
    }

    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
