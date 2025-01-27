using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MountainTracker.Core.DTO.User;
using MountainTracker.Core.Services;
using MountainTracker.Infrastructure.Entities;
using MountainTracker.Infrastructure.Repositories.Interfaces;

namespace MountainTracker.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserDto?> GetUserByIdAsync(string userId)
        {
            // convert Guid to string if needed
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Login = user.UserName,
                Nickname = user.Nickname,
                MarkerColor = user.MarkerColor
            };
        }

        public async Task UpdateUserProfileAsync(string userId, UserUpdateDto updateDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new Exception($"User {userId} not found");

            user.Nickname = updateDto.Nickname ?? user.Nickname;
            user.MarkerColor = updateDto.MarkerColor ?? user.MarkerColor;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception("Could not update user profile");
            }
        }

        public async Task ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new Exception("User not found");

            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!result.Succeeded)
            {
                throw new Exception("Could not change password");
            }
        }
    }

}
