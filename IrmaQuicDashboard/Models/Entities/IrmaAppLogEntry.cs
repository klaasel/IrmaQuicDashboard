using System;
using IrmaQuicDashboard.Models.Enums;

namespace IrmaQuicDashboard.Models.Entities
{
    public class IrmaAppLogEntry : IrmaLogEntry
    {
        public AppLogEntryType Type { get; set; }
    }
}
