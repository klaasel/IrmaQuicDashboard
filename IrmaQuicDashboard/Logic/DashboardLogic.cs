using System;
using System.Collections.Generic;
using System.Linq;
using IrmaQuicDashboard.Models.Entities;
using IrmaQuicDashboard.Models.Enums;

namespace IrmaQuicDashboard.Logic
{
    public static class DashboardLogic
    {
        public static double CalculateNewSessionToRequestIssuanceDelta(List<IrmaAppLogEntry> appLogEntries)
        {
            return CalculateTimestampDelta(appLogEntries, AppLogEntryType.RequestIssuancePermission, AppLogEntryType.NewSession);
        }

        public static double CalculateRespondToSuccessDeltaDelta(List<IrmaAppLogEntry> appLogEntries)
        {
            return CalculateTimestampDelta(appLogEntries, AppLogEntryType.Success, AppLogEntryType.RespondPermission);
        }

        public static double CalculateTimestampDelta(List<IrmaAppLogEntry> appLogEntries, AppLogEntryType endType, AppLogEntryType startType)
        {
            var endEntry = appLogEntries.Single(a => a.Type == endType);
            var startEntry = appLogEntries.Single(a => a.Type == startType);
            var diff = endEntry.Timestamp - startEntry.Timestamp;

            if (diff.Milliseconds < 0)
                throw new ArithmeticException("Difference should not be negative");
            return diff.TotalSeconds;
        }

        public static string CalculateLocationBasedOnTimestamp(DateTime timestamp, List<TimestampedLocation> locations)
        {
            if (locations == null || !locations.Any())
                return "No locations in db";

            var locs = locations.OrderBy(x => x.Timestamp);
            var latlong = locs.First(l => l.Timestamp >= timestamp);
            return $"{latlong.Longitude.ToString()},{latlong.Longitude.ToString()}";
        }

    }
}
