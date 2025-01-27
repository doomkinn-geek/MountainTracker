using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MountainTracker.Core.DTO.Message;
using MountainTracker.Core.Services;
using MountainTracker.Infrastructure.Entities;
using MountainTracker.Infrastructure.Repositories.Interfaces;

namespace MountainTracker.Infrastructure.Services
{
    public class ChatService : IChatService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatService(
            IMessageRepository messageRepository,
            IRoomRepository roomRepository,
            UserManager<ApplicationUser> userManager)
        {
            _messageRepository = messageRepository;
            _roomRepository = roomRepository;
            _userManager = userManager;
        }

        public async Task<MessageDto> SendMessageAsync(Guid roomId, string authorId, string text)
        {
            // 1) Проверяем, существует ли комната
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
                throw new Exception("Room not found");

            // 2) Вместо userRepository — используем UserManager
            var user = await _userManager.FindByIdAsync(authorId.ToString());
            if (user == null)
                throw new Exception("User not found");

            // 3) Проверяем, что userId состоит в комнате
            bool isMember = room.RoomMembers.Any(rm => rm.UserId == authorId);
            if (!isMember)
                throw new Exception("User is not a member of this room");

            // 4) Создаём сообщение
            var message = new Message
            {
                Id = Guid.NewGuid(),
                RoomId = roomId,
                AuthorId = authorId,   // храните Guid, если RoomMember.UserId — Guid
                Text = text,
                CreatedAt = DateTime.UtcNow
            };

            await _messageRepository.AddAsync(message);
            await _messageRepository.SaveChangesAsync();

            return new MessageDto
            {
                Id = message.Id,
                RoomId = message.RoomId,
                AuthorId = message.AuthorId,
                Text = message.Text,
                CreatedAt = message.CreatedAt
            };
        }

        public async Task<IEnumerable<MessageDto>> GetLastMessagesAsync(Guid roomId, int limit = 50)
        {
            var messages = await _messageRepository.GetLastMessagesAsync(roomId, limit);

            // Сортируем по возрастанию, если нужно
            return messages
                .OrderBy(m => m.CreatedAt)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    RoomId = m.RoomId,
                    AuthorId = m.AuthorId,
                    Text = m.Text,
                    CreatedAt = m.CreatedAt
                });
        }
    }
}
