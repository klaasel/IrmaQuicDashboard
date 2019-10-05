using System;
using IrmaQuicDashboard.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IrmaQuicDashboard.Models
{
    public class DashboardContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=dashboard.db");
        }

        public DbSet<IrmaSession> IrmaSessions { get; set; }
        public DbSet<IrmaAppLogEntry> AppLogEntries { get; set; }
        public DbSet<IrmaServerLogEntry> ServerLogEntries { get; set; }
        public DbSet<TimestampedLocation> TimestampedLocations { get; set; }
        public DbSet<UploadSession> UploadSessions { get; set; }
    }
}
