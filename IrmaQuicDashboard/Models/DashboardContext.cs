using System;
using IrmaQuicDashboard.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IrmaQuicDashboard.Models
{
    public class DashboardContext : DbContext
    {
        public DbSet<IrmaLogEntry> LogEntries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Logs.db");
        }
    }
}
