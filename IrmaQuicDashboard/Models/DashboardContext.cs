using System;
using IrmaQuicDashboard.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IrmaQuicDashboard.Models
{
    public class DashboardContext : DbContext
    {
        public DashboardContext(DbContextOptions<DashboardContext> options) : base(options) { }

        public DbSet<IrmaSession> IrmaSessions { get; set; }
        public DbSet<IrmaAppLogEntry> AppLogEntries { get; set; }
        public DbSet<IrmaServerLogEntry> ServerLogEntries { get; set; }
        public DbSet<TimestampedLocation> TimestampedLocations { get; set; }
        public DbSet<SessionUploadMetadata> SessionUploadMetadatas { get; set; }
    }
}
