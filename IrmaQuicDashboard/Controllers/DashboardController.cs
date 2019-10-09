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
        private readonly IUploadSessionRepository _repository;

        public DashboardController(IUploadSessionRepository repository)
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
        public IActionResult LoadSession(SessionFilterViewModel filter)
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

        public IActionResult GetTotalResult()
        {
            try
            {
                
                return PartialView("_TotalResultPartial");
            }
            catch (Exception e)
            {
                return PartialView("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorMessage = e.Message, Stacktrace = e.StackTrace });
            }
        }

        #region Helpers

        private DashboardViewModel MapUploadSessionToViewModel(UploadSession uploadSession)
        {
            var dashboardVM = new DashboardViewModel();
            dashboardVM.SessionNumber = uploadSession.SessionNumber;
            dashboardVM.UploadSessionDate = uploadSession.Date;
            dashboardVM.Location = uploadSession.Location;
            dashboardVM.Description = uploadSession.Description;
            dashboardVM.UsesQuic = uploadSession.UsesQuic;
            dashboardVM.AverageNewSessionToRequestIssuance = uploadSession.AverageNewSessionToRequestIssuance;
            dashboardVM.AverageRespondToSuccess = uploadSession.AverageRespondToSuccess;
            dashboardVM.AverageNewSessionToServerLog = uploadSession.AverageNewSessionToServerLog;
            dashboardVM.AverageServerLogToRequestIssuance = uploadSession.AverageServerLogToRequestIssuance;
            dashboardVM.AverageRespondToServerLog = uploadSession.AverageRespondToServerLog;
            dashboardVM.AverageServerLogToSuccess = uploadSession.AverageServerLogToSuccess;

            // calculate fields of IrmaSessions
            var irmaSessionSelection = uploadSession.IrmaSessions.Select
            (irmaSession => new IrmaSessionViewModel()
            {
                AppSessionId = irmaSession.AppSessionId,
                StartTime = irmaSession.Timestamp.ToLongTimeString(),
                Location = irmaSession.Location,
                NewSessionToRequestIssuanceDelta = irmaSession.NewSessionToRequestIssuanceDelta,
                RespondToSuccessDelta = irmaSession.RespondToSuccessDelta,
                NewSessionToServerLogDelta = irmaSession.NewSessionToServerLogDelta,
                ServerLogToRequestIssuanceDelta = irmaSession.ServerLogToRequestIssuanceDelta,
                RespondToServerLogDelta = irmaSession.RespondToServerLogDelta,
                ServerLogToSuccessDelta = irmaSession.ServerLogToSuccessDelta,
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
                dashboardVM.InvalidTestAmount = irmaSessionSelection.Count() - dashboardVM.ValidTestAmount;
            }

        
            return dashboardVM;
        }

        

        #endregion
    }

}
