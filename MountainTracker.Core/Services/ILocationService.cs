using MountainTracker.Core.DTO.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainTracker.Core.Services
{
    /// <summary>
    /// Интерфейс для работы с геолокацией пользователей
    /// </summary>
    public interface ILocationService
    {
        Task UpdateLocationAsync(string userId, LocationDto locationDto);
        Task<IEnumerable<LocationDto>> GetLatestLocationsForRoomAsync(Guid roomId);
    }
}
