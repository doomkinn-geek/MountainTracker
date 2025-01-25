using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainTracker.Core.DTO.Room
{
    public class RoomDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        // Список участников (Id, Nickname, ...):
        public List<UserDto> Members { get; set; } = new();
    }
}
