using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IUserRepository _userRepository;

        public ChatService(
            IMessageRepository messageRepository,
            IRoomRepository roomRepository,
            IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _roomRepository = roomRepository;
            _userRepository = userRepository;
        }

        public async Task<MessageDto> SendMessageAsync(Guid roomId, Guid authorId, string text)
        {
            // Проверяем, существует ли комната и пользователь
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
                throw new Exception("Room not found");

            var user = await _userRepository.GetByIdAsync(authorId);
            if (user == null)
                throw new Exception("User not found");

            // Можно проверить, что userId действительно состоит в этой комнате
            var isMember = room.RoomMembers.Any(rm => rm.UserId == authorId);
            if (!isMember)
                throw new Exception("User is not a member of this room");

            var message = new Message
            {
                Id = Guid.NewGuid(),
                RoomId = roomId,
                AuthorId = authorId,
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
            // Поскольку метод возвращает сообщения в порядке убывания времени, 
            // если хотим вернуть их в хронологическом порядке, можем сделать Reverse()

            return messages
                .OrderBy(m => m.CreatedAt) // сортировка по возрастанию
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
