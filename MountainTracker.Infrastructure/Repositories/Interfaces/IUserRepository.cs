using System;
using System.Threading.Tasks;
using MountainTracker.Infrastructure.Entities;

namespace MountainTracker.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByLoginAsync(string login);
        // Дополнительные методы для User
    }
}
