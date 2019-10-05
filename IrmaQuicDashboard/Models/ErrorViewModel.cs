using System;

namespace IrmaQuicDashboard.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string ErrorMessage { get; set; }

        public string Stacktrace { get; set; }

        public bool ShowErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        public bool ShowStacktrace => !string.IsNullOrEmpty(Stacktrace);

        
    }
}