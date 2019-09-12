using System;
using System.Collections.Generic;
using IrmaQuicDashboard.Models.Entities;

namespace IrmaQuicDashboard.Models.ViewModels
{
    public class DashboardViewModel
    {
        public DateTime UploadSessionDate { get; set; }
        public int SessionNumber { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public bool UsesQuic { get; set; }

        public double AverageDeltaNewSessionToRequestIssuance { get; set; }
        public double AverageDeltaRespondToSuccess { get; set; }

        public List<IrmaSessionViewModel> IrmaSessions { get; set; }

    }

    public class IrmaSessionViewModel
    {
        public int AppSessionId { get; set; }
        public string StartTime { get; set; }
        public string Location { get; set; }

        // app only
        public double NewSessionToRequestIssuanceDelta { get; set; }
        public double RespondToSuccessDelta { get; set; }

        // app and server
        public double NewSessionToServerLogDelta { get; set; }
        public double ServerLogToRequestIssuanceDelta { get; set; }
        public double RespondToServerLogDelta { get; set; }
        public double ServerLogToSuccessDelta { get; set; }
    }
}
