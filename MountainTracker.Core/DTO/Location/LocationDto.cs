using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainTracker.Core.DTO.Location
{
    public class LocationDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Altitude { get; set; }
        public double? Accuracy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
