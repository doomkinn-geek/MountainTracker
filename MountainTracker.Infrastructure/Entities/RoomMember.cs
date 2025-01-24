using System;

namespace MountainTracker.Infrastructure.Entities
{
    public class RoomMember
    {
        public Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime JoinedAt { get; set; }

        // Роль внутри комнаты (admin, user и т.д.)
        public string RoleInRoom { get; set; } = "user";
    }
}
