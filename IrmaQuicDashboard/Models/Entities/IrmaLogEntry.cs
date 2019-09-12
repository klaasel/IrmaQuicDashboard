using System;
using IrmaQuicDashboard.Models.Enums;

namespace IrmaQuicDashboard.Models.Entities
{
    public abstract class IrmaLogEntry 
    {
        public Guid Id { get; set; }
        public Guid IrmaSessionId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
