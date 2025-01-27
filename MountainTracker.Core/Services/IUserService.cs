using MountainTracker.Core.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainTracker.Core.Services
{
    /// <summary>
    /// Интерфейс для управления пользователями (профиль, настройки и т.д.)
    /// </summary>
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(string userId);
        Task UpdateUserProfileAsync(string userId, UserUpdateDto updateDto);
        Task ChangePasswordAsync(string userId, string oldPassword, string newPassword);
    }
}

