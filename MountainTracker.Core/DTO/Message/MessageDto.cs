using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainTracker.Core.DTO.Message
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public Guid AuthorId { get; set; }
        public string Text { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
