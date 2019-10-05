using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IrmaQuicDashboard.Models.Entities;

namespace IrmaQuicDashboard.Models.ViewModels
{
    public class DashboardViewModel
    {
        [Display(Name = "Upload session date: ")]
        public DateTime UploadSessionDate { get; set; }

        [Display(Name = "Upload session number: ")]
        public int SessionNumber { get; set; }

        [Display(Name = "Location: ")]
        public string Location { get; set; }

        [Display(Name = "Description: ")]
        public string Description { get; set; }

        [Display(Name = "QUIC as transport? ")]
        public bool UsesQuic { get; set; }

        [Display(Name = "Test amount: ")]
        public int TestAmount
        {
            get
            {
                return IrmaSessions.Count;
            }
        }

        public string ErrorMessage { get; set; }

        public bool ShowErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        public double AverageNewSessionToRequestIssuance { get; set; }
        public double AverageRespondToSuccess { get; set; }
        public double AverageNewSessionToServerLog { get; set; }
        public double AverageServerLogToRequestIssuance { get; set; }
        public double AverageRespondToServerLog { get; set; }
        public double AverageServerLogToSuccess { get; set; }

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
