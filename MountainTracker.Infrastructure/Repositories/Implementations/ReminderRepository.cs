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
    public class ReminderRepository : GenericRepository<Reminder>, IReminderRepository
    {
        public ReminderRepository(MountainTrackerDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Reminder>> GetRemindersForRoomAsync(Guid roomId)
        {
            return await _context.Reminders
                .Where(r => r.RoomId == roomId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reminder>> GetActiveRemindersAsync()
        {
            // Пример: все напоминания, у которых время >= текущего момента
            // (или любой другой критерий "активности")
            var now = DateTime.UtcNow;
            return await _context.Reminders
                .Where(r => r.ReminderTime >= now)
                .ToListAsync();
        }
    }
}
