using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MountainTracker.Infrastructure.Entities;

namespace MountainTracker.Infrastructure.Repositories.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<IEnumerable<Message>> GetLastMessagesAsync(Guid roomId, int limit);
    }
}
