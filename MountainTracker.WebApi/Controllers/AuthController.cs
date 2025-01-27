using Microsoft.AspNetCore.Mvc;
using MountainTracker.Core.DTO.Auth;
using MountainTracker.Core.Services;

namespace MountainTracker.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            // Пример: регистрируем пользователя
            var userId = await _authService.RegisterUserAsync(dto.Login, dto.Password, dto.Nickname);
            return Ok(new { userId });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var token = await _authService.LoginAsync(dto.Login, dto.Password);
            return Ok(new LoginResponseDto
            {
                Token = token,
                // Для удобства вернём userId
                UserId = string.Empty  // можно дополнительно узнать, чей это token, если нужно
            });
        }
    }
}
