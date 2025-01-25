using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainTracker.Core.DTO.Reminder
{
    public class ReminderDto
    {
        public Guid? Id { get; set; } // если редактирование, тогда ID
        public Guid RoomId { get; set; }
        public string Title { get; set; } = null!;
        public DateTime ReminderTime { get; set; }
        public bool IsGroupReminder { get; set; }
    }
}
