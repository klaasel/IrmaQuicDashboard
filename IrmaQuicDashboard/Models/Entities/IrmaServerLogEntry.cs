using System;
using IrmaQuicDashboard.Models.Enums;

namespace IrmaQuicDashboard.Models.Entities
{
    public class IrmaServerLogEntry : IrmaLogEntry
    {
        public ServerLogEntryType Type { get; set; }
    }
}
