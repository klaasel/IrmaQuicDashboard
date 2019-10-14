using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IrmaQuicDashboard.Models.Entities
{
    /// <summary>
    /// UploadSession contains metadata provided when uploading a session
    /// </summary>
    public class UploadSession
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public int SessionNumber { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public bool UsesQuic { get; set; }

        public bool IsStationary { get; set; }

        public bool IsWiFi { get; set; }

        public bool IsMostly4G { get; set; }

        public bool IsMostly3G { get; set; }

        public double AverageNewSessionToRequestIssuance { get; set; }

        public double AverageRespondToSuccess { get; set; }

        public double AverageNewSessionToServerLog { get; set; }

        public double AverageServerLogToRequestIssuance { get; set; }

        public double AverageRespondToServerLog { get; set; }

        public double AverageServerLogToSuccess { get; set; }

        public List<IrmaSession> IrmaSessions { get; set; }
    }
}
