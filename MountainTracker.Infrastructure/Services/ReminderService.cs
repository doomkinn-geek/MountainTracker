using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MountainTracker.Core.DTO.Reminder;
using MountainTracker.Core.Services;
using MountainTracker.Infrastructure.Entities;
using MountainTracker.Infrastructure.Repositories.Interfaces;

namespace MountainTracker.Infrastructure.Services
{
    public class ReminderService : IReminderService
    {
        private readonly IReminderRepository _reminderRepository;
        private readonly IRoomRepository _roomRepository;

        public ReminderService(IReminderRepository reminderRepository, IRoomRepository roomRepository)
        {
            _reminderRepository = reminderRepository;
            _roomRepository = roomRepository;
        }

        public async Task<Guid> CreateReminderAsync(ReminderDto reminderDto, string creatorUserId)
        {
            // Проверим, что комната существует
            var room = await _roomRepository.GetByIdAsync(reminderDto.RoomId);
            if (room == null)
                throw new Exception("Room not found");

            // Можно проверить, что создатель reminder состоит в комнате
            if (!room.RoomMembers.Any(rm => rm.UserId == creatorUserId))
                throw new Exception("Creator is not a member of this room");

            var reminder = new Reminder
            {
                Id = Guid.NewGuid(),
                RoomId = reminderDto.RoomId,
                UserId = creatorUserId,
                Title = reminderDto.Title,
                ReminderTime = reminderDto.ReminderTime,
                IsGroupReminder = reminderDto.IsGroupReminder,
                CreatedAt = DateTime.UtcNow
            };

            await _reminderRepository.AddAsync(reminder);
            await _reminderRepository.SaveChangesAsync();

            return reminder.Id;
        }

        public async Task DeleteReminderAsync(Guid reminderId, string userId)
        {
            var reminder = await _reminderRepository.GetByIdAsync(reminderId);
            if (reminder == null)
                return; // или бросить NotFoundException

            // Проверка прав: только создатель или админ может удалить
            if (reminder.UserId != userId)
            {
                // Можно проверить, не является ли userId админом комнаты...
                throw new Exception("У вас нет прав удалять этот Reminder");
            }

            _reminderRepository.Remove(reminder);
            await _reminderRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReminderDto>> GetRemindersForRoomAsync(Guid roomId)
        {
            var reminders = await _reminderRepository.GetRemindersForRoomAsync(roomId);
            return reminders.Select(r => new ReminderDto
            {
                Id = r.Id,
                RoomId = r.RoomId,
                Title = r.Title,
                ReminderTime = r.ReminderTime,
                IsGroupReminder = r.IsGroupReminder
            });
        }
    }
}
