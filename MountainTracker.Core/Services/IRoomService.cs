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
        Task<Guid> CreateRoomAsync(RoomCreateDto dto, Guid creatorUserId);
        Task<bool> JoinRoomAsync(Guid roomId, Guid userId, string? roomPassword = null);
        Task LeaveRoomAsync(Guid roomId, Guid userId);
        Task<RoomDto?> GetRoomInfoAsync(Guid roomId);
        Task<IEnumerable<RoomDto>> GetRoomsForUserAsync(Guid userId);        
    }
}

