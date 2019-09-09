using System;
namespace IrmaQuicDashboard.Models.Entities
{
    public class TimestampedLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTimeOffset Timestamp { get; set; }

    }
}
