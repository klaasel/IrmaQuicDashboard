using System;
using System.Collections.Generic;
using IrmaQuicDashboard.Models.Entities;

namespace IrmaQuicDashboard.Repository
{
    public interface IDashboardRepository
    {
        List<SessionUploadMetadata> GetUploadSessions();
        SessionUploadMetadata GetUploadSession(Guid id);
    }
}
