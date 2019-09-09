using System;
using IrmaQuicDashboard.Models.Enums;

namespace IrmaQuicDashboard.Models.Entities
{
    public class IrmaLogEntry
    {
        public int Id { get; set; }
        public int IrmaLogSessionId { get; set; }
        public DateTime Timestamp { get; set; }
        public LogEntrySource Source {get;set;}
        public LogEntryType Type { get; set; }
        public string RawInfo { get; set; }
        public string RawJson { get; set; }
    }
}
