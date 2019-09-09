using System;
using IrmaQuicDashboard.Models.Enums;

namespace IrmaQuicDashboard.Models.Entities
{
    public class IrmaLogEntry
    {
        public DateTimeOffset Timestamp { get; set; }
        public LogEntrySource Source {get;set;}
        public LogEntryType Type { get; set; }
    }

    
}
