using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainTracker.Core.DTO.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public string UserId { get; set; }
    }
}
