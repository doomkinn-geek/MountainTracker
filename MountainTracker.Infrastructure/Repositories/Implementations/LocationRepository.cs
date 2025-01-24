using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MountainTracker.Infrastructure.Data;
using MountainTracker.Infrastructure.Entities;
using MountainTracker.Infrastructure.Repositories.Interfaces;

namespace MountainTracker.Infrastructure.Repositories.Implementations
{
    public class LocationRepository : GenericRepository<Location>, ILocationRepository
    {
        public LocationRepository(MountainTrackerDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Location>> GetLatestLocationsForRoomAsync(Guid roomId)
        {
            // Пример: берем последних Location от каждого участника конкретной комнаты
            // (в реальном коде можно усложнить SQL-запрос/группировку)

            // Сначала найдём всех UserId в комнате:
            var userIds = await _context.RoomMembers
                .Where(rm => rm.RoomId == roomId)
                .Select(rm => rm.UserId)
                .Distinct()
                .ToListAsync();

            // Для каждого участника выберем самую свежую запись:
            var latestLocations = new List<Location>();

            foreach (var userId in userIds)
            {
                var loc = await _dbSet
                    .Where(l => l.UserId == userId)
                    .OrderByDescending(l => l.CreatedAt)
                    .FirstOrDefaultAsync();

                if (loc != null)
                    latestLocations.Add(loc);
            }

            return latestLocations;
        }
    }
}
