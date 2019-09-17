using System;
using Microsoft.AspNetCore.Http;

namespace IrmaQuicDashboard.Repository
{
    public interface IServerLogEntriesRepository
    {
        bool ProcessServerLog(IFormFile serverLog, Guid sessionMetadataId);
    }
}
