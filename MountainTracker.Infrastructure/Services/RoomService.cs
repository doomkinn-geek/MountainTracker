using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MountainTracker.Core.DTO.Room;
using MountainTracker.Core.DTO.User;
using MountainTracker.Core.Services;
using MountainTracker.Infrastructure.Entities;
using MountainTracker.Infrastructure.Repositories.Interfaces;

namespace MountainTracker.Infrastructure.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoomService(IRoomRepository roomRepository, UserManager<ApplicationUser> userManager)
        {
            _roomRepository = roomRepository;
            _userManager = userManager;
        }

        public async Task<Guid> CreateRoomAsync(RoomCreateDto dto, string creatorUserId)
        {
            // 1) Получаем пользователя через UserManager
            var creator = await _userManager.FindByIdAsync(creatorUserId.ToString());
            if (creator == null)
                throw new Exception("Creator not found");

            // 2) Создаём объект Room
            var room = new Room
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                PasswordHash = dto.Password != null ? HashPassword(dto.Password) : null,
                // NOTE: room.CreatedByUserId — если это Guid, ок. Но creator.Id может быть string.
                // Если нужно хранить его как Guid, настраиваем IdentityUser<Guid>. 
                CreatedByUserId = creatorUserId,
                IsPublic = dto.IsPublic,
                CreatedAt = DateTime.UtcNow
            };

            await _roomRepository.AddAsync(room);
            await _roomRepository.SaveChangesAsync();

            // 3) Добавляем участника
            var roomMember = new RoomMember
            {
                RoomId = room.Id,
                UserId = creatorUserId, // Guid or string, depending on your design
                RoleInRoom = "admin",
                JoinedAt = DateTime.UtcNow
            };
            room.RoomMembers.Add(roomMember);
            await _roomRepository.SaveChangesAsync();

            return room.Id;
        }

        public async Task<bool> JoinRoomAsync(Guid roomId, string userId, string? roomPassword = null)
        {
            // 1) Ищем комнату
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
                throw new Exception("Room not found");

            // 2) Проверка пароля (если есть)
            if (room.PasswordHash != null)
            {
                var passwordOk = VerifyPassword(roomPassword ?? "", room.PasswordHash);
                if (!passwordOk)
                    throw new Exception("Пароль неверный");
            }

            // 3) Проверим, не вступал ли пользователь уже
            var alreadyMember = room.RoomMembers.Any(rm => rm.UserId == userId);
            if (alreadyMember)
            {
                // уже в комнате
                return true;
            }

            // 4) Добавляем участника
            var newMember = new RoomMember
            {
                RoomId = room.Id,
                UserId = userId,
                RoleInRoom = "user",
                JoinedAt = DateTime.UtcNow
            };
            room.RoomMembers.Add(newMember);

            _roomRepository.Update(room);
            await _roomRepository.SaveChangesAsync();

            return true;
        }

        public async Task LeaveRoomAsync(Guid roomId, string userId)
        {
            // 1) Ищем комнату
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
                throw new Exception("Room not found");

            // 2) Ищем участника
            var member = room.RoomMembers.FirstOrDefault(rm => rm.UserId == userId);
            if (member == null)
            {
                // Не состоим в комнате
                return;
            }

            room.RoomMembers.Remove(member);
            _roomRepository.Update(room);
            await _roomRepository.SaveChangesAsync();
        }

        public async Task<RoomDto?> GetRoomInfoAsync(Guid roomId)
        {
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
                return null;

            // Загрузим участников (возможно, нужно Include(r => r.RoomMembers).ThenInclude(u => u.User) в репозитории)
            var membersDto = room.RoomMembers
                .Select(rm => new UserDto
                {
                    Id = rm.UserId,
                    // Логин и Nickname можно взять через rm.User, если сделан Include
                    Nickname = rm.User?.Nickname,
                    MarkerColor = rm.User?.MarkerColor
                })
                .ToList();

            return new RoomDto
            {
                Id = room.Id,
                Title = room.Title,
                IsPublic = room.IsPublic,
                CreatedAt = room.CreatedAt,
                Members = membersDto
            };
        }

        public async Task<IEnumerable<RoomDto>> GetRoomsForUserAsync(string userId)
        {
            var rooms = await _roomRepository.GetRoomsForUserAsync(userId);

            // Преобразуем в DTO
            return rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                Title = r.Title,
                IsPublic = r.IsPublic,
                CreatedAt = r.CreatedAt,
                Members = r.RoomMembers.Select(rm => new UserDto
                {
                    Id = rm.UserId,
                    Nickname = rm.User?.Nickname,
                    MarkerColor = rm.User?.MarkerColor
                }).ToList()
            });
        }

        // -------------- HELPER METHODS --------------

        private string HashPassword(string pwd)
        {
            return "hashed_" + pwd;
        }

        private bool VerifyPassword(string? input, string storedHash)
        {
            if (input == null) return false;
            return storedHash == ("hashed_" + input);
        }
    }
}
