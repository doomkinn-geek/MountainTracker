using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MountainTracker.Core.DTO.Reminder;
using MountainTracker.Core.Services;

namespace MountainTracker.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReminderController : ControllerBase
    {
        private readonly IReminderService _reminderService;

        public ReminderController(IReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReminder([FromBody] ReminderDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var reminderId = await _reminderService.CreateReminderAsync(dto, userId);
            return Ok(new { reminderId });
        }

        [HttpDelete("{reminderId}")]
        public async Task<IActionResult> DeleteReminder(Guid reminderId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            await _reminderService.DeleteReminderAsync(reminderId, userId);
            return NoContent();
        }

        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetRemindersForRoom(Guid roomId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // Можно проверить, что userId состоит в комнате...
            var reminders = await _reminderService.GetRemindersForRoomAsync(roomId);
            return Ok(reminders);
        }

        private string? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return null;
            return userIdClaim.Value;
        }
    }
}
