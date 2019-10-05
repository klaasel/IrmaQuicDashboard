using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IrmaQuicDashboard.Logic;
using IrmaQuicDashboard.Models;
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
            try
            {
                var uploadSession = _repository.GetUploadSession(filter.Id);
                var dashboardVM = MapUploadSessionToViewModel(uploadSession);
                return PartialView("_DashboardPartial", dashboardVM);
            }
            catch (Exception e)
            {
                return PartialView("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorMessage = e.Message, Stacktrace = e.StackTrace });
            }

            
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
            var irmaSessionSelection = uploadSession.IrmaSessions.Select
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
            });
            var filteredIrmaSessionProjection = irmaSessionSelection
                // filter the projection on criterium: app delta should be more or equal as sum of the corresponding app-server deltas.
                .Where(x =>
                    x.NewSessionToRequestIssuanceDelta >= 0 &&
                    (x.NewSessionToServerLogDelta + x.ServerLogToRequestIssuanceDelta <= x.NewSessionToRequestIssuanceDelta) &&
                    x.RespondToSuccessDelta >= 0 &&
                    (x.RespondToServerLogDelta + x.ServerLogToSuccessDelta <= x.RespondToSuccessDelta)
                    );

            if (!filteredIrmaSessionProjection.Any())
            {
                // app and server logs calculations do not match! show them anyway with warning: corrupt data.
                dashboardVM.IrmaSessions = irmaSessionSelection
                    .OrderBy(sessions => sessions.StartTime)
                    .ToList();
                dashboardVM.ErrorMessage = "The app and server logs calculations do not match!";
            } else
            {
                dashboardVM.IrmaSessions = filteredIrmaSessionProjection
                .OrderBy(sessions => sessions.StartTime)
                .ToList();
            }

            // calculate averages
            dashboardVM.AverageNewSessionToRequestIssuance = Math.Round(dashboardVM.IrmaSessions.Select(i => i.NewSessionToRequestIssuanceDelta).Average(),3, MidpointRounding.AwayFromZero);
            dashboardVM.AverageRespondToSuccess = Math.Round(dashboardVM.IrmaSessions.Select(i => i.RespondToSuccessDelta).Average(), 3, MidpointRounding.AwayFromZero);
            dashboardVM.AverageNewSessionToServerLog = Math.Round(dashboardVM.IrmaSessions.Select(i => i.NewSessionToServerLogDelta).Average(), 3, MidpointRounding.AwayFromZero);
            dashboardVM.AverageServerLogToRequestIssuance = Math.Round(dashboardVM.IrmaSessions.Select(i => i.ServerLogToRequestIssuanceDelta).Average(), 3, MidpointRounding.AwayFromZero);
            dashboardVM.AverageRespondToServerLog = Math.Round(dashboardVM.IrmaSessions.Select(i => i.RespondToServerLogDelta).Average(), 3, MidpointRounding.AwayFromZero);
            dashboardVM.AverageServerLogToSuccess = Math.Round(dashboardVM.IrmaSessions.Select(i => i.ServerLogToSuccessDelta).Average(), 3, MidpointRounding.AwayFromZero);

            return dashboardVM;
        }

        

        #endregion
    }

}
