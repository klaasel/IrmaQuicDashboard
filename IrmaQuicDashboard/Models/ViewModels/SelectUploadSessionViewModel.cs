using System;
using System.Collections.Generic;
using IrmaQuicDashboard.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IrmaQuicDashboard.Models.ViewModels
{
    public class SelectUploadSessionViewModel
    {
        public Guid UploadSessionId { get; set; }

        // These are all uploaded sessions.
        public List<SelectListItem> SessionSelection { get; set; }



    }
}
