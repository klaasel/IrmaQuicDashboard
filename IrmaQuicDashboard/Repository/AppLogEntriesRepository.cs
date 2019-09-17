using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IrmaQuicDashboard.Extensions;
using IrmaQuicDashboard.Models;
using IrmaQuicDashboard.Models.Entities;
using IrmaQuicDashboard.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace IrmaQuicDashboard.Repository
{
    public class AppLogEntriesRepository : IAppLogEntriesRepository
    {
        private readonly DashboardContext _context;
        private IrmaSession _currentIrmaSession;

        public AppLogEntriesRepository(DashboardContext context)
        {
            _context = context;
        }

        public SessionUploadMetadata CreateNewUploadSession(DateTime date, string location, string description, bool usesQuic, int sessionNumberUploaded, IFormFile applog)
        {
            var success = false;
            var sessionMetadata = new SessionUploadMetadata()
            {
                Id = Guid.NewGuid(),
                Date = date,
                Location = location,
                Description = description,
                UsesQuic = usesQuic,
                SessionNumber = sessionNumberUploaded
            };

            // save metadata
            _context.Add(sessionMetadata);
            _context.SaveChanges();

            // convert the app log
            success = ConvertAndSaveAppLog(applog, sessionMetadata.Id);

            // reset the currentIrmaSession
            _currentIrmaSession = null;
            return sessionMetadata;
        }

        private bool ConvertAndSaveAppLog(IFormFile file, Guid sessionMetadataId)
        {
            // because a lot of lines need to be filtered, don't save them in memory but immediately write them to the database
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                {
                    var line = reader.ReadLine();
                    ProcessAppLogLine(line, sessionMetadataId);
                }
            }
            return true;
        }

        private bool ProcessAppLogLine(string line, Guid sessionMetadataId)
        {
            // only process Position if a session is active
            if (line.Contains("Position") && _currentIrmaSession != null)
            {
                ProcessTimestampedLocation(line);
            }
            if (line.Contains("NewSession"))
            {
                ProcessNewIrmaSessionAndLogEntry(line, sessionMetadataId);
            }
            if (line.Contains("RequestIssuancePermission"))
            {
                ProcessAppLogEntry(line, AppLogEntryType.RequestIssuancePermission);
            }
            if (line.Contains("RespondPermission"))
            {
                ProcessAppLogEntry(line, AppLogEntryType.RespondPermission);
            }
            if (line.Contains("Success"))
            {
                ProcessAppLogEntry(line, AppLogEntryType.Success);
            }

            // if the line does not contain one of these, just continue.
            return true;
        }

        private bool ProcessTimestampedLocation(string line)
        {
            var timestampedLocation = new TimestampedLocation();

            // deconstruct line: [<timestamp>]| Position: |<lat>,<long>
            var lineParts = line.Split('|');
            var coords = lineParts[2].Split(',');

            // validate
            if (lineParts.Length != 3 || coords.Length != 2)
                throw new ArgumentException(nameof(lineParts));

            // assign
            timestampedLocation.IrmaSessionId = _currentIrmaSession.Id;
            timestampedLocation.Timestamp = DateTime.Parse(lineParts[0].Trim('[').Trim(']'));
            timestampedLocation.Latitude = Double.Parse(coords[0]);
            timestampedLocation.Longitude = Double.Parse(coords[1]);

            // save
            _context.Add(timestampedLocation);
            _context.SaveChanges();
            return true;
        }

        private bool ProcessNewIrmaSessionAndLogEntry(string line, Guid sessionMetadataId)
        {
            // deconstruct line: [<timestamp>]| Sending ..: | JSON object
            var lineParts = line.Split('|');
            var timestamp = DateTime.Parse(lineParts[0].Trim('[').Trim(']'));
            dynamic json = JsonConvert.DeserializeObject<dynamic>(lineParts[2].Trim());
            string url = json.request.u;
            string sessionToken = url.After("/irma/");

            // create new IrmaSession
            var irmaSession = new IrmaSession();
            irmaSession.Id = Guid.NewGuid();
            irmaSession.SessionUploadMetadataId = sessionMetadataId;
            irmaSession.Timestamp = timestamp;
            irmaSession.AppSessionId = json.sessionId;
            irmaSession.SessionToken = sessionToken;

            // create the log entry
            var appLogEntry = new IrmaAppLogEntry();
            appLogEntry.Id = Guid.NewGuid();
            appLogEntry.Timestamp = timestamp;
            appLogEntry.Type = AppLogEntryType.NewSession;
            appLogEntry.IrmaSessionId = irmaSession.Id;

            // keep track of the new IrmaSession for future lines
            _currentIrmaSession = irmaSession;

            // add and save
            _context.Add(irmaSession);
            _context.Add(appLogEntry);
            _context.SaveChanges();

            return true;
        }

        private bool ProcessAppLogEntry(string line, AppLogEntryType type)
        {
            if (type == AppLogEntryType.NewSession)
                throw new ArgumentException("Use ProcessNewIrmaSessionAndLogEntry method", nameof(type));

            // deconstruct line: [<timestamp>]| Receiving ..: | JSON object 
            var lineParts = line.Split('|');
            var timestamp = DateTime.Parse(lineParts[0].Trim('[').Trim(']'));
            dynamic json = JsonConvert.DeserializeObject<dynamic>(lineParts[2].Trim());

            // check the sessionId
            int sessionId = json.sessionId;
            if (sessionId == 0 || sessionId !=_currentIrmaSession.AppSessionId)
                return false;

            // create the log entry
            var appLogEntry = new IrmaAppLogEntry();
            appLogEntry.Id = Guid.NewGuid();
            appLogEntry.Timestamp = timestamp;
            appLogEntry.Type = type;
            appLogEntry.IrmaSessionId = _currentIrmaSession.Id;

            // add and save
            _context.Add(appLogEntry);
            _context.SaveChanges();

            return true;
        }
    }
}
