using System;
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
                        throw new InvalidDataException("Processing server log unsuccessful: " + line);
                }
            }

            // reset current irma session for the next upload session
            _currentIrmaSession = null;
            return true;
        }

        private bool ProcessServerLogLine(string line, Guid sessionMetadataId)
        {
            if (line.Contains("sessionPtr"))
            {
                // 1. Process the sessionpointer (from irmajs (TRACE HTTP JSON response: 200 {"sessionPtr":{"u": etc):
                ProcessAndUpdateIrmaSession(line, sessionMetadataId);
            }
            if (line.Contains("DEBUG Routing protocol message method=GET path=irma/") && !line.EndsWith("status", StringComparison.CurrentCulture))
            {
                // 2. Process the GET path/<token> log message and connect it to irma session (DEBUG Routing protocol message method=GET path=irma/<token>)
                ProcessServerLogEntry(line, ServerLogEntryType.ServerLogGETIrmaWithToken);
            }
            if (line.Contains("TRACE HTTP JSON response: 200 {\"context\":"))
            {
                // 3. Process response of path/<token> (TRACE HTTP JSON response: 200 {"context":"AQ==", etc..)
                ProcessServerLogEntry(line, ServerLogEntryType.ServerLogJSONResponseIssuingCredentials);
            }
            if (line.Contains("commitments"))
            {
                // 4. Process the POST commitments (DEBUG Routing protocol message method=POST path=irma/<token>/commitments
                ProcessServerLogEntry(line, ServerLogEntryType.ServerLogPOSTCommitments);
            }
            if (line.Contains("TRACE HTTP JSON response: 200 [{\"proof\":{\"c\":"))
            {
                // 5. Process the response (TRACE HTTP JSON response: 200 [{"proof":{"c": etc)
                ProcessServerLogEntry(line, ServerLogEntryType.ServerLogJSONResponseProof);
            }

            // if the line does not contain one of these, just continue.
            return true;
        }

        private bool ProcessAndUpdateIrmaSession(string line, Guid sessionMetadataId)
        {
            // deconstruct line: [<timestamp>] TRACE HTTP JSON response: 200 {"sessionPtr":{"u": ... }
            var timestamp = DateTime.Parse(line.Between("[", "]"));
            dynamic json = JsonConvert.DeserializeObject<dynamic>(line.After("200 ").Trim());
            string url = json.sessionPtr.u;
            string sessionToken = url.After("/irma/");
            var irmaJsSessionToken = json.token;

            // get the corresponding irma session of this upload session and its entries
            var irmaSession = _context.IrmaSessions
                .Where(sessions => sessions.SessionUploadMetadataId == sessionMetadataId)
                .Include(sessions => sessions.AppLogEntries)
                .FirstOrDefault(session => session.SessionToken == sessionToken);

            // validate existence 
            if (irmaSession == null)
                return false;

            var successAppLogEntry = irmaSession.AppLogEntries.FirstOrDefault(x => x.Type == AppLogEntryType.Success);

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
            var timestamp = DateTime.Parse(line.Between("[", "] "));

            if (type == ServerLogEntryType.ServerLogGETIrmaWithToken)
            {
                // validate if token is the same
                var sessionToken = line.After("path=irma/");
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
