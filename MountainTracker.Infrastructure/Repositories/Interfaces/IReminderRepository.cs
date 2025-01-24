using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MountainTracker.Infrastructure.Entities;

namespace MountainTracker.Infrastructure.Repositories.Interfaces
{
    public interface IReminderRepository : IGenericRepository<Reminder>
    {
        Task<IEnumerable<Reminder>> GetRemindersForRoomAsync(Guid roomId);
        Task<IEnumerable<Reminder>> GetActiveRemindersAsync();
    }
}
