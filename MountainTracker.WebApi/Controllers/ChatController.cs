using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MountainTracker.Core.DTO.Message;
using MountainTracker.Core.Services;

namespace MountainTracker.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("rooms/{roomId}")]
        public async Task<IActionResult> SendMessage(Guid roomId, [FromBody] SendMessageRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var msg = await _chatService.SendMessageAsync(roomId, userId, request.Text);
            return Ok(msg);
        }

        [HttpGet("rooms/{roomId}/messages")]
        public async Task<IActionResult> GetMessages(Guid roomId, [FromQuery] int limit = 50)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // Можно проверять, состоит ли userId в комнате, и т.д.
            var messages = await _chatService.GetLastMessagesAsync(roomId, limit);
            return Ok(messages);
        }

        private string? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return null;
            return userIdClaim.Value;            
        }
    }

    // Вспомогательный класс для запроса
    public class SendMessageRequest
    {
        public string Text { get; set; } = null!;
    }
}
