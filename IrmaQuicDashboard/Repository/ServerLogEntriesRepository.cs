using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using IrmaQuicDashboard.Extensions;
using IrmaQuicDashboard.Models;
using IrmaQuicDashboard.Models.Entities;
using IrmaQuicDashboard.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace IrmaQuicDashboard.Repository
{
    public class ServerLogEntriesRepository : IServerLogEntriesRepository
    {
        private readonly DashboardContext _context;
        private IrmaSession _currentIrmaSession;

        public ServerLogEntriesRepository(DashboardContext context)
        {
            _context = context;
        }

        public bool ProcessServerLog(IFormFile serverLog, Guid sessionMetadataId)
        {
            using (var reader = new StreamReader(serverLog.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                {
                    var line = reader.ReadLine();
                    var success = ProcessServerLogLine(line, sessionMetadataId);
                    if (!success)
                        throw new LogProcessingException("Processing server log unsuccessful at line: " + line);
                }
            }

            // reset current irma session for the next upload session
            _currentIrmaSession = null;
            return true;
        }

        private bool ProcessServerLogLine(string line, Guid sessionMetadataId)
        {
            // in order of appearance:
            if (line.Contains("sessionPtr"))
            {
                // 1. Process the sessionpointer from irmajs (TRACE <= response duration=16.2826ms response={"sessionPtr" etc):
                // this will set the _currentIrmaSession
                return ProcessAndUpdateIrmaSession(line, sessionMetadataId);
            }
            if (_currentIrmaSession != null)
            {
                if (line.Contains("TRACE => request") &&
                    line.Contains("url=/irma/") &&
                    !line.EndsWith("status", StringComparison.CurrentCulture) &&
                    !line.EndsWith("commitments", StringComparison.CurrentCulture))
                {
                    // 2. Process the GET path/<token> log message (TRACE => request headers=map ... url=/irma/token)
                    return ProcessServerLogEntry(line, ServerLogEntryType.ServerLogGETIrmaWithToken);
                }
                if (line.Contains("TRACE <= response") && line.Contains("issuance"))
                {
                    // 3. Process response of irma/<token> (TRACE <= response duration=0s response={"@context": etc)
                    return ProcessServerLogEntry(line, ServerLogEntryType.ServerLogJSONResponseIssuingCredentials);
                }
                if (line.Contains("TRACE => request") && line.Contains("commitments"))
                {
                    // 4. Process the POST commitments 
                    return ProcessServerLogEntry(line, ServerLogEntryType.ServerLogPOSTCommitments);
                }
                if (line.Contains("TRACE <= response") && line.Contains("{\"proof\":{\"c\":"))
                {
                    // 5. Process the response (TRACE <= response duration=31.2084ms response=[{"proof":{"c": etc.)
                    return ProcessServerLogEntry(line, ServerLogEntryType.ServerLogJSONResponseProof);
                }
            }

            // if the line does not contain one of these, just continue.
            return true;
        }

        private bool ProcessAndUpdateIrmaSession(string line, Guid sessionMetadataId)
        {
            // deconstruct line: [<timestamp>] TRACE HTTP JSON response: 200 {"sessionPtr":{"u": ... }
            var timestamp = DateTime.Parse(line.Between("[", "]"));
            dynamic json = JsonConvert.DeserializeObject<dynamic>(line.Between("response=", " status=200").Trim());
            string url = json.sessionPtr.u;
            string sessionToken = url.After("/irma/");
            var irmaJsSessionToken = json.token;

            // get the corresponding irma session of this upload session and its entries
            var irmaSession = _context.IrmaSessions
                .Where(sessions => sessions.UploadSessionId == sessionMetadataId)
                .Include(sessions => sessions.AppLogEntries)
                .FirstOrDefault(session => session.SessionToken == sessionToken);

            // validate existence, if not continue with the next session
            if (irmaSession == null)
            {
                _currentIrmaSession = null;
                Debug.WriteLine("No corresponding irma session found for line: " + line);
                return true;
            }


            var successAppLogEntry = irmaSession.AppLogEntries.FirstOrDefault(x => x.Type == AppLogEntryType.Success);

            if (successAppLogEntry == null)
            {
                // incomplete session, don't use it but continue
                _currentIrmaSession = null;
                Debug.WriteLine("No successAppLogEntry found for line: " + line);
                return true;
            }
            // validate session has not finished yet
            if (timestamp >= successAppLogEntry.Timestamp)
            {
                return false;
            }

            // update session with js session token for connecting data
            irmaSession.IrmaJsSessionToken = irmaJsSessionToken;

            // keep track of the irma session for future lines
            _currentIrmaSession = irmaSession;

            _context.SaveChanges();

            return true;
        }

        private bool ProcessServerLogEntry(string line, ServerLogEntryType type)
        {
            var timestamp = DateTime.Parse(line.Between("[", "] TRACE"));

            if (type == ServerLogEntryType.ServerLogGETIrmaWithToken)
            {
                // validate if token is the same
                var sessionToken = line.Between("url=/irma/", "/");
                if (sessionToken != _currentIrmaSession.SessionToken)
                    return false;
            }

            if (type == ServerLogEntryType.ServerLogPOSTCommitments)
            {
                // validate if token is the same
                var sessionToken = line.Between("url=/irma/", "/commitments");
                if (sessionToken != _currentIrmaSession.SessionToken)
                    return false;
            }

            // create the server log entry
            var serverLogEntry = new IrmaServerLogEntry
            {
                Id = Guid.NewGuid(),
                Timestamp = timestamp,
                Type = type,
                IrmaSessionId = _currentIrmaSession.Id
            };

            // add and save
            _context.Add(serverLogEntry);
            _context.SaveChanges();
            return true;
        }
    }
}
