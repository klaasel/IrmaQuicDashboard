using System;
using System.Collections.Generic;
using System.Linq;
using IrmaQuicDashboard.Models;
using IrmaQuicDashboard.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IrmaQuicDashboard.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly DashboardContext _context;
        public DashboardRepository(DashboardContext context)
        {
            _context = context;
        }

        public UploadSession GetUploadSession(Guid id)
        {
            var session =
                _context.UploadSessions
                    .Include(uploads => uploads.IrmaSessions)
                        .ThenInclude(irmaSession => irmaSession.AppLogEntries)
                    .Include(uploads => uploads.IrmaSessions)
                        .ThenInclude(irmaSession => irmaSession.TimestampedLocations)
                    .Include(uploads => uploads.IrmaSessions)
                        .ThenInclude(irmaSession => irmaSession.ServerLogEntries)
                   .SingleOrDefault(x => x.Id == id);

            return session;
        }

        public List<UploadSession> GetUploadSessions()
        {
            var sessions = _context
                .UploadSessions
                .ToList();
            return sessions;
        }
    }
}
