using MountainTracker.Core.DTO.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainTracker.Core.Services
{
    /// <summary>
    /// Интерфейс для чат-функционала (обмена сообщениями)
    /// </summary>
    public interface IChatService
    {
        Task<MessageDto> SendMessageAsync(Guid roomId, string authorId, string text);
        Task<IEnumerable<MessageDto>> GetLastMessagesAsync(Guid roomId, int limit = 50);
    }
}
