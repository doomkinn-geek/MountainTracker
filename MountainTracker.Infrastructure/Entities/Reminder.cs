using System;

namespace MountainTracker.Infrastructure.Entities
{
    public class Reminder
    {
        public Guid Id { get; set; }

        public Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public Guid UserId { get; set; }  // кто создал
        public User? User { get; set; }

        public string Title { get; set; } = null!;

        public DateTime ReminderTime { get; set; }

        public bool IsGroupReminder { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
