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

        public SessionUploadMetadata GetUploadSession(Guid id)
        {
            var session =
                _context.SessionUploadMetadatas
                    .Include(uploads => uploads.IrmaSessions)
                        .ThenInclude(irmaSession => irmaSession.AppLogEntries)
                    .Include(uploads => uploads.IrmaSessions)
                        .ThenInclude(irmaSession => irmaSession.TimestampedLocations)
                   .SingleOrDefault(x => x.Id == id);

            return session;
        }

        public List<SessionUploadMetadata> GetUploadSessions()
        {
            var sessions = _context
                .SessionUploadMetadatas
                .ToList();
            return sessions;
        }
    }
}
