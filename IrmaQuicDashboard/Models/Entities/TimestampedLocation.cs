using System;
namespace IrmaQuicDashboard.Models.Entities
{
    public class TimestampedLocation
    {
        public int Id { get; set; }
        public int IrmaLogSessionId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
