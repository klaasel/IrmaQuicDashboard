using System;
using System.Collections.Generic;
using System.Linq;
using IrmaQuicDashboard.Models.Entities;
using IrmaQuicDashboard.Models.Enums;

namespace IrmaQuicDashboard.Logic
{
    public static class DashboardLogic
    {
        #region AppLog

        public static double CalculateNewSessionToRequestIssuanceDelta(List<IrmaAppLogEntry> appLogEntries)
        {
            return CalculateApplogTimestampDelta(appLogEntries, AppLogEntryType.NewSession, AppLogEntryType.RequestIssuancePermission);
        }

        public static double CalculateRespondToSuccessDelta(List<IrmaAppLogEntry> appLogEntries)
        {
            return CalculateApplogTimestampDelta(appLogEntries, AppLogEntryType.RespondPermission, AppLogEntryType.Success);
        }

        private static double CalculateApplogTimestampDelta(List<IrmaAppLogEntry> appLogEntries, AppLogEntryType startType, AppLogEntryType endType)
        {
            if (!appLogEntries.Any())
            {
                // there is something missing, indicate by -1
                return -1.0;
            }

            var endEntry = appLogEntries.Single(a => a.Type == endType);
            var startEntry = appLogEntries.Single(a => a.Type == startType);
            var diff = endEntry.Timestamp - startEntry.Timestamp;

            if (diff.Milliseconds < 0)
                throw new ArithmeticException("Difference should not be negative");
            return diff.TotalSeconds;
        }

        #endregion

        #region AppToServerLog

        public static double CalculateNewSessionToServerLogDelta(List<IrmaAppLogEntry> appLogEntries, List<IrmaServerLogEntry> serverLogEntries)
        {
            return CalculateAppToServerLogsTimestampDelta(appLogEntries, serverLogEntries, AppLogEntryType.NewSession, ServerLogEntryType.ServerLogGETIrmaWithToken);
        }

        public static double CalculateServerLogToRequestIssuanceDelta(List<IrmaAppLogEntry> appLogEntries, List<IrmaServerLogEntry> serverLogEntries)
        {
            return CalculateServerToAppLogsTimestampDelta(appLogEntries, serverLogEntries, ServerLogEntryType.ServerLogJSONResponseIssuingCredentials, AppLogEntryType.RequestIssuancePermission);
        }

        public static double CalculateRespondToServerLogDelta(List<IrmaAppLogEntry> appLogEntries, List<IrmaServerLogEntry> serverLogEntries)
        {
            return CalculateAppToServerLogsTimestampDelta(appLogEntries, serverLogEntries, AppLogEntryType.RespondPermission, ServerLogEntryType.ServerLogPOSTCommitments);
        }

        public static double CalculateServerLogToSuccessDelta(List<IrmaAppLogEntry> appLogEntries, List<IrmaServerLogEntry> serverLogEntries)
        {
            return CalculateServerToAppLogsTimestampDelta(appLogEntries, serverLogEntries, ServerLogEntryType.ServerLogJSONResponseProof, AppLogEntryType.Success);
        }

        private static double CalculateAppToServerLogsTimestampDelta(List<IrmaAppLogEntry> appLogEntries, List<IrmaServerLogEntry> serverLogEntries, AppLogEntryType startType, ServerLogEntryType endType)
        {
            if (!serverLogEntries.Any() || !appLogEntries.Any())
            {
                // there is something missing, indicate by -1
                return -1.0;
            }

            var endEntry = serverLogEntries.Single(a => a.Type == endType);
            var startEntry = appLogEntries.Single(a => a.Type == startType);
            var diff = endEntry.Timestamp - startEntry.Timestamp;

            if (diff.Milliseconds < 0)
                throw new ArithmeticException("Difference should not be negative");
            return diff.TotalSeconds;
        }

        private static double CalculateServerToAppLogsTimestampDelta(List<IrmaAppLogEntry> appLogEntries, List<IrmaServerLogEntry> serverLogEntries, ServerLogEntryType startType, AppLogEntryType endType)
        {
            if (!serverLogEntries.Any() || !appLogEntries.Any())
            {
                // there is something missing, indicate by -1
                return -1.0;
            }
            var endEntry = appLogEntries.Single(a => a.Type == endType);
            var startEntry = serverLogEntries.Single(a => a.Type == startType);
            var diff = endEntry.Timestamp - startEntry.Timestamp;

            if (diff.Milliseconds < 0)
                throw new ArithmeticException("Difference should not be negative");
            return diff.TotalSeconds;
        }

        #endregion


        #region Location

        public static string CalculateLocationBasedOnTimestamp(DateTime timestamp, List<TimestampedLocation> locations)
        {
            if (locations == null || !locations.Any())
                return "No locations in db";

            var locs = locations.OrderBy(x => x.Timestamp);
            var latlong = locs.First(l => l.Timestamp >= timestamp);
            return $"{latlong.Latitude.ToString()},{latlong.Longitude.ToString()}";
        }

        #endregion

    }
}
