using System;
using System.Collections.Generic;
using IrmaQuicDashboard.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IrmaQuicDashboard.Models.ViewModels
{
    public class DashboardViewModel
    {
        public Guid UploadSessionId { get; set; }

        // WIP: this is for the enum to select the right session
        // it should be filled with all possible UploadSession
        public List<SelectListItem> SessionSelection { get; set; }



    }
}
