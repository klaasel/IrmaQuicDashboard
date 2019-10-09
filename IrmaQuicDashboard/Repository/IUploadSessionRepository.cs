using System;
using System.Collections.Generic;
using IrmaQuicDashboard.Models.Entities;

namespace IrmaQuicDashboard.Repository
{
    public interface IUploadSessionRepository
    {
        List<UploadSession> GetUploadSessions();
        UploadSession GetUploadSession(Guid id);
        bool UpdateUploadSession(UploadSession uploadSession);
    }
}
