using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MountainTracker.Core.Services;
using MountainTracker.Infrastructure.Entities;
using MountainTracker.Infrastructure.Repositories.Interfaces;

namespace MountainTracker.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<Guid> RegisterUserAsync(string login, string password, string nickname)
        {
            // 1) Проверка, нет ли уже пользователя с таким логином
            var existingUser = await _userRepository.GetByLoginAsync(login);
            if (existingUser != null)
            {
                throw new Exception("Пользователь с таким логином уже существует");
                // можно бросить NotFoundException или ValidationException
            }

            // 2) Создаём нового пользователя
            var user = new User
            {
                Id = Guid.NewGuid(),
                Login = login,
                PasswordHash = HashPassword(password),
                Nickname = nickname,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return user.Id;
        }

        public async Task<string> LoginAsync(string login, string password)
        {
            // 1) Ищем пользователя по логину
            var user = await _userRepository.GetByLoginAsync(login);
            if (user == null)
            {
                throw new Exception("Неверный логин или пароль");
            }

            // 2) Проверяем пароль
            if (!VerifyPassword(password, user.PasswordHash))
            {
                throw new Exception("Неверный логин или пароль");
            }

            // 3) Генерируем JWT
            var token = GenerateJwtToken(user.Id);
            return token;
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

        private string GenerateJwtToken(Guid userId)
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
