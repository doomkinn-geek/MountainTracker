using System;
using System.Threading.Tasks;
using MountainTracker.Core.DTO.User;
using MountainTracker.Core.Services;
using MountainTracker.Infrastructure.Entities;
using MountainTracker.Infrastructure.Repositories.Interfaces;

namespace MountainTracker.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Login = user.Login,
                Nickname = user.Nickname,
                MarkerColor = user.MarkerColor
            };
        }

        public async Task UpdateUserProfileAsync(Guid userId, UserUpdateDto updateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception($"User with id {userId} not found.");
            }

            user.Nickname = updateDto.Nickname ?? user.Nickname;
            user.MarkerColor = updateDto.MarkerColor ?? user.MarkerColor;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception($"User with id {userId} not found.");

            // Допустим, мы знаем, как "верифицировать" хеш
            if (!VerifyPassword(oldPassword, user.PasswordHash))
                throw new Exception("Старый пароль не совпадает.");

            user.PasswordHash = HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        // ---------------- HELPER METHODS ----------------

        private string HashPassword(string password)
        {
            return "hashed_" + password;
        }

        private bool VerifyPassword(string plainPassword, string storedHash)
        {
            return storedHash == ("hashed_" + plainPassword);
        }
    }
}
