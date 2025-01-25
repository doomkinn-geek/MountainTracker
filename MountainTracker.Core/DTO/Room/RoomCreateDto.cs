using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainTracker.Core.DTO.Room
{
    public class RoomCreateDto
    {
        public string Title { get; set; } = null!;
        public string? Password { get; set; }
        public bool IsPublic { get; set; }
    }
}
