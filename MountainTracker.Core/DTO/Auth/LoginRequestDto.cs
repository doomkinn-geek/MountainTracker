﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainTracker.Core.DTO.Auth
{
    public class LoginRequestDto
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
