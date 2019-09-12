using System;
namespace IrmaQuicDashboard.Models.Entities
{
    public class TimestampedLocation 
    {
        public Guid Id { get; set; }
        public Guid IrmaSessionId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
