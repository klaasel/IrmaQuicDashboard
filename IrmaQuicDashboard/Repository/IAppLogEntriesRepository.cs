using System;
using System.Collections.Generic;
using IrmaQuicDashboard.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace IrmaQuicDashboard.Repository
{
    public interface IAppLogEntriesRepository
    {
        UploadSession CreateNewUploadSession(DateTime date,
            string location,
            string description,
            bool usesQuic,
            bool isStationary,
            bool isWifi,
            bool isMostly4G,
            bool isMostly3G,
            int sessionNumberUploaded,
            IFormFile applog);
       
    }
}
