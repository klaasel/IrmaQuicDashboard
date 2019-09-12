using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IrmaQuicDashboard.Models.Entities
{
    /// <summary>
    /// Metadata provided when uploading a session
    /// </summary>
    public class SessionUploadMetadata
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public int SessionNumber { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public bool UsesQuic { get; set; }

        public List<IrmaSession> IrmaSessions { get; set; }
    }
}
