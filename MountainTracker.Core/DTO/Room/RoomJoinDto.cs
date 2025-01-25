using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainTracker.Core.DTO.Room
{
    public class RoomJoinDto
    {
        public Guid RoomId { get; set; }
        public string? Password { get; set; }
    }
}
