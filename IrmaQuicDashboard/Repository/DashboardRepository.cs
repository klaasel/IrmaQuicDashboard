using System;
using System.Collections.Generic;
using System.Linq;
using IrmaQuicDashboard.Models;
using IrmaQuicDashboard.Models.Entities;

namespace IrmaQuicDashboard.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly DashboardContext _context;
        public DashboardRepository(DashboardContext context)
        {
            _context = context;
        }

        public List<SessionUploadMetadata> GetUploadSessions()
        {
            var sessions = _context.SessionUploadMetadatas.ToList();
            return sessions;
        }
    }
}
