using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainTracker.Core.DTO.User
{
    public class UserDto
    {
        public string Id { get; set; }
        public string? Login { get; set; }
        public string? Nickname { get; set; }
        public string? MarkerColor { get; set; }
    }
}
