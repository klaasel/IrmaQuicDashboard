using System;
using System.Collections.Generic;
using System.Linq;
using IrmaQuicDashboard.Models.Entities;
using IrmaQuicDashboard.Models.Enums;

namespace IrmaQuicDashboard.Logic
{
    public static class CalculationLogic
    {
        #region AppLog

        public static double CalculateNewSessionToRequestIssuanceDelta(List<IrmaAppLogEntry> appLogEntries)
        {
            return RoundToThreeDecimals(CalculateApplogTimestampDelta(appLogEntries, AppLogEntryType.NewSession, AppLogEntryType.RequestIssuancePermission));
        }

        public static double CalculateRespondToSuccessDelta(List<IrmaAppLogEntry> appLogEntries)
        {
            return RoundToThreeDecimals(CalculateApplogTimestampDelta(appLogEntries, AppLogEntryType.RespondPermission, AppLogEntryType.Success));
        }

        private static double CalculateApplogTimestampDelta(List<IrmaAppLogEntry> appLogEntries, AppLogEntryType startType, AppLogEntryType endType)
        {
            if (!appLogEntries.Any())
            {
                // there is something missing, indicate by -1
                return -1.0;
            }

            var endEntry = appLogEntries.FirstOrDefault(a => a.Type == endType);
            var startEntry = appLogEntries.FirstOrDefault(a => a.Type == startType);

            if (endEntry == null || startEntry == null )
            {
                // there is something missing, indicate by -1
                return -1.0;
            }


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

            var endEntry = serverLogEntries.FirstOrDefault(a => a.Type == endType);
            var startEntry = appLogEntries.FirstOrDefault(a => a.Type == startType);

            if (endEntry == null || startEntry == null)
            {
                // there is something missing, indicate by -1
                return -1.0;
            }

            var diff = endEntry.Timestamp - startEntry.Timestamp;
            return diff.TotalSeconds;
        }

        private static double CalculateServerToAppLogsTimestampDelta(List<IrmaAppLogEntry> appLogEntries, List<IrmaServerLogEntry> serverLogEntries, ServerLogEntryType startType, AppLogEntryType endType)
        {
            if (!serverLogEntries.Any() || !appLogEntries.Any())
            {
                // there is something missing, indicate by -1
                return -1.0;
            }
            var endEntry = appLogEntries.FirstOrDefault(a => a.Type == endType);
            var startEntry = serverLogEntries.FirstOrDefault(a => a.Type == startType);
            var diff = endEntry.Timestamp - startEntry.Timestamp;

            if (endEntry == null || startEntry == null)
            {
                // there is something missing, indicate by -1
                return -1.0;
            }
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

        #region Upload session averages

        public static UploadSession CalculateAverages(UploadSession uploadSession, List<IrmaSession> irmaSessions)
        {
            uploadSession.AverageNewSessionToRequestIssuance = RoundToThreeDecimals(irmaSessions.Select(i => i.NewSessionToRequestIssuanceDelta).Average());
            uploadSession.AverageRespondToSuccess = RoundToThreeDecimals(irmaSessions.Select(i => i.RespondToSuccessDelta).Average());
            uploadSession.AverageNewSessionToServerLog = RoundToThreeDecimals(irmaSessions.Select(i => i.NewSessionToServerLogDelta).Average());
            uploadSession.AverageServerLogToRequestIssuance = RoundToThreeDecimals(irmaSessions.Select(i => i.ServerLogToRequestIssuanceDelta).Average());
            uploadSession.AverageRespondToServerLog = RoundToThreeDecimals(irmaSessions.Select(i => i.RespondToServerLogDelta).Average());
            uploadSession.AverageServerLogToSuccess = RoundToThreeDecimals(irmaSessions.Select(i => i.ServerLogToSuccessDelta).Average());
            return uploadSession;
        }

        #endregion

        #region Rounding helper

        public static double RoundToThreeDecimals(double value)
        {
            return Math.Round(value, 3, MidpointRounding.AwayFromZero);
        }

        #endregion

    }
}
