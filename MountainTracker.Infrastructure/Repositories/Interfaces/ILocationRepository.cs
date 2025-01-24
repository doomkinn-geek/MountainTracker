using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MountainTracker.Infrastructure.Entities;

namespace MountainTracker.Infrastructure.Repositories.Interfaces
{
    public interface ILocationRepository : IGenericRepository<Location>
    {
        Task<IEnumerable<Location>> GetLatestLocationsForRoomAsync(Guid roomId);
        // Пр. выборка последних координат каждого участника
    }
}
