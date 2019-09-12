using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace IrmaQuicDashboard.Repository
{
    public interface ILogEntriesRepository
    {
        bool CreateNewUploadSession(DateTime date, string location, string description, bool usesQuic, int sessionNumberUploaded, IFormFile applog, IFormFile serverLog);
       
    }
}
