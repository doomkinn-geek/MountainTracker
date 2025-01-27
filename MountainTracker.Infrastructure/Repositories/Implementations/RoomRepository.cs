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
    public class RoomRepository : GenericRepository<Room>, IRoomRepository
    {
        public RoomRepository(MountainTrackerDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Room>> GetRoomsForUserAsync(string userId)
        {
            // Пример: выбираем комнаты, где userId состоит в RoomMembers
            return await _context.Rooms
                .Include(r => r.RoomMembers)
                .Where(r => r.RoomMembers.Any(m => m.UserId == userId))
                .ToListAsync();
        }
    }
}
