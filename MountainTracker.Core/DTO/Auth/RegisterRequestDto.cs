using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainTracker.Core.DTO.Auth
{
    public class RegisterRequestDto
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Nickname { get; set; } = null!;
    }
}
