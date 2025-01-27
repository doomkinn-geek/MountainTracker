using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MountainTracker.Core.DTO.Room;
using MountainTracker.Core.Services;

namespace MountainTracker.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] RoomCreateDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var roomId = await _roomService.CreateRoomAsync(dto, userId);
            return Ok(new { roomId });
        }

        [HttpPost("{roomId}/join")]
        public async Task<IActionResult> JoinRoom([FromRoute] Guid roomId, [FromBody] RoomJoinDto joinDto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _roomService.JoinRoomAsync(roomId, userId, joinDto.Password);
            if (!result) return BadRequest("Не удалось войти в комнату");
            return Ok();
        }

        [HttpPost("{roomId}/leave")]
        public async Task<IActionResult> LeaveRoom([FromRoute] Guid roomId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            await _roomService.LeaveRoomAsync(roomId, userId);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetMyRooms()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var rooms = await _roomService.GetRoomsForUserAsync(userId);
            return Ok(rooms);
        }

        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetRoomInfo([FromRoute] Guid roomId)
        {
            var roomInfo = await _roomService.GetRoomInfoAsync(roomId);
            if (roomInfo == null) return NotFound();
            return Ok(roomInfo);
        }

        private string? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return null;
            return userIdClaim.Value;
        }
    }
}
