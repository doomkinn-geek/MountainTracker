using System;

namespace MountainTracker.Infrastructure.Entities
{
    public class Message
    {
        public Guid Id { get; set; }

        public Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public string AuthorId { get; set; }
        public ApplicationUser Author { get; set; } = null!;

        public string Text { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
