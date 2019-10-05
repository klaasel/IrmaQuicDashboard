using System;
using System.Collections.Generic;
using IrmaQuicDashboard.Models.Entities;

namespace IrmaQuicDashboard.Repository
{
    public interface IDashboardRepository
    {
        List<UploadSession> GetUploadSessions();
        UploadSession GetUploadSession(Guid id);
    }
}
