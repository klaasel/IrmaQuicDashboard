﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using IrmaQuicDashboard.Logic;

namespace IrmaQuicDashboard.Models.Entities
{
    /// <summary>
    /// A single irma session logged to app and server
    /// </summary>
    public class IrmaSession
    {
        public Guid Id { get; set; }

        public Guid UploadSessionId { get; set; }

        /// <summary>
        /// External Id generated by the app, alternate key
        /// </summary>
        public int AppSessionId { get; set; }

        /// <summary>
        /// Session token, which is part of the QR Code path
        /// </summary>
        public string SessionToken { get; set; }

        /// <summary>
        /// Session token, which is generated by the requestor (Irmajs)
        /// </summary>
        public string IrmaJsSessionToken { get; set; }

        /// <summary>
        /// Timestamp as provided by the app 'NewSession' message log in UTC.
        /// </summary>
        public DateTime Timestamp { get; set; }

        [NotMapped]
        public string StartTime
        {
            get
            {
                return Timestamp.ToLongDateString();
            }
        }

        /// <summary>
        /// Location as provided periodically by the GPS in the mobile device
        /// </summary>
        [NotMapped]
        public string Location {
            get
            {
                return CalculationLogic.CalculateLocationBasedOnTimestamp(Timestamp, TimestampedLocations);
            }
        }

        /// <summary>
        /// Difference between the NewSession and RequestIssuance IRMA message types
        /// </summary>
        [NotMapped]
        public double NewSessionToRequestIssuanceDelta
        {
            get
            {
                return CalculationLogic.CalculateNewSessionToRequestIssuanceDelta(AppLogEntries);
            }
        }

        /// <summary>
        /// Difference between the RespondPermission and Success IRMA message types
        /// </summary>
        [NotMapped]
        public double RespondToSuccessDelta
        {
            get
            {
                return CalculationLogic.CalculateRespondToSuccessDelta(AppLogEntries);
            }
        }

        // Above messages with their corresponding server logs
        [NotMapped]
        public double NewSessionToServerLogDelta
        {
            get
            {
                return CalculationLogic.CalculateNewSessionToServerLogDelta(AppLogEntries, ServerLogEntries);
            }
        }

        [NotMapped]
        public double ServerLogToRequestIssuanceDelta
        {
            get
            {
                return CalculationLogic.CalculateServerLogToRequestIssuanceDelta(AppLogEntries, ServerLogEntries);
            }
        }

        [NotMapped]
        public double RespondToServerLogDelta
        {
            get
            {
                return CalculationLogic.CalculateRespondToServerLogDelta(AppLogEntries, ServerLogEntries);
            }
        }

        [NotMapped]
        public double ServerLogToSuccessDelta
        {
            get
            {
                return CalculationLogic.CalculateServerLogToSuccessDelta(AppLogEntries, ServerLogEntries);
            }
        }

        public List<IrmaAppLogEntry> AppLogEntries { get; set; }
        public List<IrmaServerLogEntry> ServerLogEntries { get; set; }
        public List<TimestampedLocation> TimestampedLocations { get; set; }
    }
}
