using MountainTracker.Core.DTO.Reminder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainTracker.Core.Services
{
    /// <summary>
    /// Интерфейс для управления напоминаниями (будильники/групповые уведомления)
    /// </summary>
    public interface IReminderService
    {
        Task<Guid> CreateReminderAsync(ReminderDto reminder, string creatorUserId);
        Task DeleteReminderAsync(Guid reminderId, string userId);
        Task<IEnumerable<ReminderDto>> GetRemindersForRoomAsync(Guid roomId);
    }
}
