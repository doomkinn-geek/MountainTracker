using System;

namespace MountainTracker.Infrastructure.Entities
{
    public class Location
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Altitude { get; set; }
        public double? Accuracy { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
