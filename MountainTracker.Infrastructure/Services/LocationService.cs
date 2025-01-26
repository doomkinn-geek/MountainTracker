using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MountainTracker.Core.DTO.Location;
using MountainTracker.Core.Services;
using MountainTracker.Infrastructure.Entities;
using MountainTracker.Infrastructure.Repositories.Interfaces;

namespace MountainTracker.Infrastructure.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IRoomRepository _roomRepository;

        public LocationService(ILocationRepository locationRepository, IRoomRepository roomRepository)
        {
            _locationRepository = locationRepository;
            _roomRepository = roomRepository;
        }

        public async Task UpdateLocationAsync(Guid userId, LocationDto locationDto)
        {
            // Создаём запись локации
            var loc = new Location
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Latitude = locationDto.Latitude,
                Longitude = locationDto.Longitude,
                Altitude = locationDto.Altitude,
                Accuracy = locationDto.Accuracy,
                CreatedAt = DateTime.UtcNow
            };

            await _locationRepository.AddAsync(loc);
            await _locationRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<LocationDto>> GetLatestLocationsForRoomAsync(Guid roomId)
        {
            // Используем специальный метод репозитория, который выбирает 
            // самую свежую координату для каждого участника
            var locations = await _locationRepository.GetLatestLocationsForRoomAsync(roomId);

            var result = new List<LocationDto>();
            foreach (var loc in locations)
            {
                result.Add(new LocationDto
                {
                    Latitude = loc.Latitude,
                    Longitude = loc.Longitude,
                    Altitude = loc.Altitude,
                    Accuracy = loc.Accuracy,
                    CreatedAt = loc.CreatedAt
                });
            }
            return result;
        }
    }
}
