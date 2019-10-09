using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IrmaQuicDashboard.Repository;
using IrmaQuicDashboard.Models;
using IrmaQuicDashboard.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IrmaQuicDashboard.Logic;

namespace IrmaQuicDashboard.Controllers
{
    public class IrmaLogSessionUploadController : Controller
    {
        private readonly IAppLogEntriesRepository _appLogRepository;
        private readonly IServerLogEntriesRepository _serverLogRepository;
        private readonly IUploadSessionRepository _uploadSessionRepository;

        public IrmaLogSessionUploadController(
            IAppLogEntriesRepository appLogRepository,
            IServerLogEntriesRepository serverLogRepository,
            IUploadSessionRepository uploadSessionRepository)
        {
            _appLogRepository = appLogRepository;
            _serverLogRepository = serverLogRepository;
            _uploadSessionRepository = uploadSessionRepository;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        // POST: a new session
        [HttpPost]
        public IActionResult CreateLog(IrmaLogSessionUploadViewModel model)
        {
            
            if (model == null)
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            try
            {
                var uploadSession = _appLogRepository.CreateNewUploadSession(
                    model.Date,
                    model.Location,
                    model.Description,
                    model.UsesQuic,
                    model.IsStationary,
                    model.IsWiFi,
                    model.IsMostly4G,
                    model.IsMostly3G,
                    model.SessionNumberUploaded,
                    model.AppLog
                );

                _serverLogRepository.ProcessServerLog(model.ServerLog, uploadSession.Id);

                // Get the valid irma sessions, calculate the averages from those sessions and save them to the uploadsession
                var uploadSessionEntity = _uploadSessionRepository.GetUploadSession(uploadSession.Id);
                var validIrmaSessions = uploadSessionEntity.IrmaSessions.Where(x =>
                    x.NewSessionToRequestIssuanceDelta >= 0 &&
                    (x.NewSessionToServerLogDelta + x.ServerLogToRequestIssuanceDelta <= x.NewSessionToRequestIssuanceDelta) &&
                    x.RespondToSuccessDelta >= 0 &&
                    (x.RespondToServerLogDelta + x.ServerLogToSuccessDelta <= x.RespondToSuccessDelta)
                    )
                    .ToList();

                uploadSessionEntity = CalculationLogic.CalculateAverages(uploadSession, validIrmaSessions);
                _uploadSessionRepository.UpdateUploadSession(uploadSessionEntity);

            }
            catch (LogProcessingException e)
            {
                return View("Error",new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorMessage = e.Message });
            }

            return RedirectToAction("Index");
        }

        

        
    }
}
