using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainTracker.Core.DTO.User
{
    public class UserCreateDto
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Nickname { get; set; }
        public string? MarkerColor { get; set; }
    }
}
