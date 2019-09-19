using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IrmaQuicDashboard.Logic;
using IrmaQuicDashboard.Models.Entities;
using IrmaQuicDashboard.Models.Enums;
using IrmaQuicDashboard.Models.ViewModels;
using IrmaQuicDashboard.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IrmaQuicDashboard.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _repository;

        public DashboardController(IDashboardRepository repository)
        {
            _repository = repository;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var viewModel = new SelectUploadSessionViewModel();
            // get all possible sessions for dropdown
            var sessions = _repository.GetUploadSessions();
            viewModel.SessionSelection =
                sessions.Select(session => new SelectListItem()
                {
                    Value = session.Id.ToString(),
                    Text = session.Date.ToShortDateString() + ", nr: " + session.SessionNumber
                })
                 .ToList();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult UploadSession(SessionFilterViewModel filter)
        {
            var uploadSession = _repository.GetUploadSession(filter.Id);
            var dashboardVM = MapUploadSessionToViewModel(uploadSession);
            return PartialView("_DashboardPartial", dashboardVM);
        }


        #region Helpers

        private DashboardViewModel MapUploadSessionToViewModel(SessionUploadMetadata uploadSession)
        {
            var dashboardVM = new DashboardViewModel();
            dashboardVM.SessionNumber = uploadSession.SessionNumber;
            dashboardVM.UploadSessionDate = uploadSession.Date;
            dashboardVM.Location = uploadSession.Location;
            dashboardVM.Description = uploadSession.Description;
            dashboardVM.UsesQuic = uploadSession.UsesQuic;

            // calculate fields of IrmaSessions
            dashboardVM.IrmaSessions = uploadSession.IrmaSessions.Select
            (irmaSession => new IrmaSessionViewModel()
            {
                AppSessionId = irmaSession.AppSessionId,
                StartTime = irmaSession.Timestamp.ToLongTimeString(),
                Location = DashboardLogic.CalculateLocationBasedOnTimestamp(irmaSession.Timestamp, irmaSession.TimestampedLocations),
                NewSessionToRequestIssuanceDelta = DashboardLogic.CalculateNewSessionToRequestIssuanceDelta(irmaSession.AppLogEntries),
                RespondToSuccessDelta = DashboardLogic.CalculateRespondToSuccessDelta(irmaSession.AppLogEntries),
                NewSessionToServerLogDelta = DashboardLogic.CalculateNewSessionToServerLogDelta(irmaSession.AppLogEntries, irmaSession.ServerLogEntries),
                ServerLogToRequestIssuanceDelta = DashboardLogic.CalculateServerLogToRequestIssuanceDelta(irmaSession.AppLogEntries, irmaSession.ServerLogEntries),
                RespondToServerLogDelta = DashboardLogic.CalculateRespondToServerLogDelta(irmaSession.AppLogEntries, irmaSession.ServerLogEntries),
                ServerLogToSuccessDelta = DashboardLogic.CalculateServerLogToSuccessDelta(irmaSession.AppLogEntries, irmaSession.ServerLogEntries),
            })
            .OrderBy(sessions => sessions.StartTime)
            .ToList();

            // calculate averages
            dashboardVM.AverageNewSessionToRequestIssuance = dashboardVM.IrmaSessions.Select(i => i.NewSessionToRequestIssuanceDelta).Average();
            dashboardVM.AverageRespondToSuccess = dashboardVM.IrmaSessions.Select(i => i.RespondToSuccessDelta).Average();
            dashboardVM.AverageNewSessionToServerLog = dashboardVM.IrmaSessions.Select(i => i.NewSessionToServerLogDelta).Average();
            dashboardVM.AverageServerLogToRequestIssuance = dashboardVM.IrmaSessions.Select(i => i.ServerLogToRequestIssuanceDelta).Average();
            dashboardVM.AverageRespondToServerLog = dashboardVM.IrmaSessions.Select(i => i.RespondToServerLogDelta).Average();
            dashboardVM.AverageServerLogToSuccess = dashboardVM.IrmaSessions.Select(i => i.ServerLogToSuccessDelta).Average();

            return dashboardVM;
        }

        

        #endregion
    }

}
