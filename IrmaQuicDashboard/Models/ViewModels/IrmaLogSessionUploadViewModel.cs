using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace IrmaQuicDashboard.Models.ViewModels
{
    public class IrmaLogSessionUploadViewModel
    {
        [Required]
        public int SessionNumberUploaded { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Description { get; set; }

        public bool UsesQuic { get; set; }

        [Required]
        public IFormFile AppLog { get; set; }

        [Required]
        public IFormFile ServerLog { get; set; }
    }
}
