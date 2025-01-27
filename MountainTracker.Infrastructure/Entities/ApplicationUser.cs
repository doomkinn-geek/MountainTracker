using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace MountainTracker.Infrastructure.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? Nickname { get; set; }

        public string? MarkerColor { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Навигационное свойство: какие комнаты у пользователя
        public ICollection<RoomMember> RoomMembers { get; set; } = new List<RoomMember>();
    }
}
