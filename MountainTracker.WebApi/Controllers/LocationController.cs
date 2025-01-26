using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MountainTracker.Core.DTO.Location;
using MountainTracker.Core.Services;

namespace MountainTracker.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLocation([FromBody] LocationDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            await _locationService.UpdateLocationAsync(userId.Value, dto);
            return Ok();
        }

        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetLatestLocations(Guid roomId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // (опционально) можно проверять, состоит ли userId в комнате
            var locations = await _locationService.GetLatestLocationsForRoomAsync(roomId);
            return Ok(locations);
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return null;
            if (Guid.TryParse(userIdClaim.Value, out var guidVal))
                return guidVal;
            return null;
        }
    }
}
