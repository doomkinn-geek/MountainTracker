using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MountainTracker.Core.Services;
using MountainTracker.Infrastructure.Entities;
using MountainTracker.Infrastructure.Repositories.Interfaces;

namespace MountainTracker.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = config;
        }

        public async Task<Guid> RegisterUserAsync(string login, string password, string nickname)
        {
            // Создаём нового пользователя
            var user = new ApplicationUser
            {
                UserName = login,
                Email = login,
                Nickname = nickname,
                CreatedAt = DateTime.UtcNow
                // MarkerColor и пр. поля
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new Exception($"Не удалось создать пользователя: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // Возвращаем Guid. Но, если ключ User это string, нужно конвертировать.
            return Guid.Parse(user.Id);
        }

        public async Task<string> LoginAsync(string login, string password)
        {
            var user = await _userManager.FindByNameAsync(login);
            if (user == null)
                throw new Exception("Неверный логин или пароль");

            // Проверяем пароль
            var passwordOk = await _userManager.CheckPasswordAsync(user, password);
            if (!passwordOk)
                throw new Exception("Неверный логин или пароль");

            // Генерируем JWT (как раньше)
            return GenerateJwtToken(user.Id);
        }

        public Task<Guid?> ValidateTokenAsync(string token)
        {
            // Пример валидации JWT
            try
            {
                var validationParameters = GetTokenValidationParameters();
                var tokenHandler = new JwtSecurityTokenHandler();

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                if (principal.Identity is ClaimsIdentity identity)
                {
                    // Извлекаем claim "sub" = userId
                    var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                    {
                        return Task.FromResult<Guid?>(userId);
                    }
                }
            }
            catch
            {
                // ignore or log
            }

            return Task.FromResult<Guid?>(null);
        }

        // ---------------- HELPER METHODS ----------------

        private string HashPassword(string password)
        {
            // Здесь должен использоваться надёжный алгоритм (BCrypt, PBKDF2, Argon2, ...).
            // Для примера используем просто "hashed_" + password
            return "hashed_" + password;
        }

        private bool VerifyPassword(string plainPassword, string storedHash)
        {
            // Сравниваем с "hashed_" + plainPassword:
            return storedHash == "hashed_" + plainPassword;
        }

        private string GenerateJwtToken(string userId)
        {
            var secretKey = _configuration["Jwt:SecretKey"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                // можно добавить ClaimTypes.Name, ClaimTypes.Role и т.п.
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(4),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            var secretKey = _configuration["Jwt:SecretKey"];
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            };
        }
    }
}
