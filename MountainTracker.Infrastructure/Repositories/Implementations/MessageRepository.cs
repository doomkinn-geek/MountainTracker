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
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(MountainTrackerDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Message>> GetLastMessagesAsync(Guid roomId, int limit)
        {
            // Выбираем последние limit сообщений по времени
            return await _context.Messages
                .Where(m => m.RoomId == roomId)
                .OrderByDescending(m => m.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }
    }
}
