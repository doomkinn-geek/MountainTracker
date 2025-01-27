using MountainTracker.Core.DTO.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainTracker.Core.Services
{
    /// <summary>
    /// Интерфейс для работы с комнатами (создание, вступление, выход)
    /// </summary>
    public interface IRoomService
    {
        Task<Guid> CreateRoomAsync(RoomCreateDto dto, string creatorUserId);
        Task<bool> JoinRoomAsync(Guid roomId, string userId, string? roomPassword = null);
        Task LeaveRoomAsync(Guid roomId, string userId);
        Task<RoomDto?> GetRoomInfoAsync(Guid roomId);
        Task<IEnumerable<RoomDto>> GetRoomsForUserAsync(string userId);        
    }
}

