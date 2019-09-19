using System;
namespace IrmaQuicDashboard
{
    public class LogProcessingException : Exception
    {
        public LogProcessingException()
        {
        }

        public LogProcessingException(string message)
            : base(message)
        {
        }

        public LogProcessingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
