using System;
using System.Collections.Generic;
using IrmaQuicDashboard.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace IrmaQuicDashboard.Repository
{
    public interface IAppLogEntriesRepository
    {
        SessionUploadMetadata CreateNewUploadSession(DateTime date, string location, string description, bool usesQuic, int sessionNumberUploaded, IFormFile applog);
       
    }
}
