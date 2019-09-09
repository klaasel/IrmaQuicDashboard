using System;
using System.Collections.Generic;

namespace IrmaQuicDashboard.Models.Entities
{
    public class IrmaLogSession
    {
        public int Id { get; set; }
        public int SessionNumber { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

        public List<IrmaLogEntry> IrmaLogEntries { get; set; }
        public List<TimestampedLocation> TimestampedLocations { get; set; }
    }
}
