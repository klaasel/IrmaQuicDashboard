using System;
using System.Collections.Generic;
using System.Linq;
using IrmaQuicDashboard.Models.Entities;

namespace IrmaQuicDashboard.Logic
{
    public static class IrmaSessionLogic
    {
        public static List<IrmaSession> FilterValidIrmaSessions(List<IrmaSession> irmaSessions)
        {
            var validIrmaSessions = irmaSessions.Where(x =>
                    x.NewSessionToRequestIssuanceDelta >= 0 &&
                    (x.NewSessionToServerLogDelta + x.ServerLogToRequestIssuanceDelta <= x.NewSessionToRequestIssuanceDelta) &&
                    x.RespondToSuccessDelta >= 0 &&
                    (x.RespondToServerLogDelta + x.ServerLogToSuccessDelta <= x.RespondToSuccessDelta)
                    )
                    .ToList();

            return validIrmaSessions;
        }
    }
}
