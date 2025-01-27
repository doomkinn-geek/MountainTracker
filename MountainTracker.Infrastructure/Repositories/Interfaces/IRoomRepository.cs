using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MountainTracker.Infrastructure.Entities;

namespace MountainTracker.Infrastructure.Repositories.Interfaces
{
    public interface IRoomRepository : IGenericRepository<Room>
    {
        Task<IEnumerable<Room>> GetRoomsForUserAsync(string userId);
        // Другие методы, специфичные для Room
    }
}
