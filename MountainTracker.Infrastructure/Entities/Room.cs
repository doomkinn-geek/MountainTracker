using System;
using System.Collections.Generic;

namespace MountainTracker.Infrastructure.Entities
{
    public class Room
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string? PasswordHash { get; set; }

        public string CreatedByUserId { get; set; }

        public bool IsPublic { get; set; }

        public DateTime CreatedAt { get; set; }

        // Навигационное свойство: список участников
        public ICollection<RoomMember> RoomMembers { get; set; } = new List<RoomMember>();

        // Навигационное свойство: сообщения чата
        public ICollection<Message> Messages { get; set; } = new List<Message>();

        // Напоминания, связанные с этой комнатой
        public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
    }
}
