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
                    Text = session.Date.ToShortDateString() + ", nr: " +
                   ", " + session.SessionNumber +
                   ", " + (session.UsesQuic ? "QUIC" : " HTTP/TLS")  +
                    ", " + ComposeTestmodeText(session)
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
                var uploadSessions = _repository.GetUploadSessions();
                var totalResultViewModel = MapUploadSessionsToTotalResultViewModel(uploadSessions);

                return PartialView("_TotalResultPartial", totalResultViewModel);
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
            dashboardVM.Testmode = ComposeTestmodeText(uploadSession);
            dashboardVM.AverageNewSessionToRequestIssuance = uploadSession.AverageNewSessionToRequestIssuance;
            dashboardVM.AverageRespondToSuccess = uploadSession.AverageRespondToSuccess;
            dashboardVM.AverageNewSessionToServerLog = uploadSession.AverageNewSessionToServerLog;
            dashboardVM.AverageServerLogToRequestIssuance = uploadSession.AverageServerLogToRequestIssuance;
            dashboardVM.AverageRespondToServerLog = uploadSession.AverageRespondToServerLog;
            dashboardVM.AverageServerLogToSuccess = uploadSession.AverageServerLogToSuccess;

            // Project calculated fields of IrmaSessions.
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

            // Filter the projection on criterium: app delta should be more or equal as sum of the corresponding app-server deltas.
            var filteredIrmaSessionProjection = irmaSessionSelection
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
                dashboardVM.ErrorMessage = "The app and server logs calculations do not match, this should not happen! Showing them for reference";
            } else
            {
                dashboardVM.IrmaSessions = filteredIrmaSessionProjection
                .OrderBy(sessions => sessions.StartTime)
                .ToList();
                dashboardVM.InvalidTestAmount = irmaSessionSelection.Count() - dashboardVM.ValidTestAmount;
            }

        
            return dashboardVM;
        }

        private string ComposeTestmodeText(UploadSession uploadSession)
        {
            string text = "";
            text += uploadSession.IsWiFi ? "WiFi, " : "";
            text += uploadSession.IsMostly3G ? "3G, " : "";
            text += uploadSession.IsMostly4G ? "4G, " : "";
            text += uploadSession.IsStationary ? "stationary" : "moving";
            text = text.Trim(',').Trim();
            return text;
        }

        private TotalResultViewModel MapUploadSessionsToTotalResultViewModel(List<UploadSession> uploadSessions)
        {
            var viewModel = new TotalResultViewModel();

            // we need to split up the uploadSessions
            var wifiUploadSessionsQUIC = uploadSessions.Where(u => u.IsWiFi && u.UsesQuic).ToList();
            var stationary3GUploadSessionsQUIC = uploadSessions.Where(u => u.IsMostly3G && u.IsStationary && u.UsesQuic).ToList();
            var moving4GUploadSessionsQUIC = uploadSessions.Where(u => u.IsMostly4G && !u.IsStationary && u.UsesQuic).ToList();
            var wifiUploadSessionsTLS = uploadSessions.Where(u => u.IsWiFi && !u.UsesQuic).ToList();
            var stationary3GUploadSessionsTLS = uploadSessions.Where(u => u.IsMostly3G && u.IsStationary && !u.UsesQuic).ToList();
            var moving4GUploadSessionsTLS = uploadSessions.Where(u => u.IsMostly4G && !u.IsStationary && !u.UsesQuic).ToList();

            // assign averages when comparing sessions exist
            if (wifiUploadSessionsQUIC.Any() && wifiUploadSessionsTLS.Any())
            {
                viewModel.QuicUsingWiFiNewRip = wifiUploadSessionsQUIC.Average(u => u.AverageNewSessionToRequestIssuance);
                viewModel.TLSUsingWiFiNewRip = wifiUploadSessionsTLS.Average(u => u.AverageNewSessionToRequestIssuance);
                viewModel.QuicUsingWiFiResSuc = wifiUploadSessionsQUIC.Average(u => u.AverageRespondToSuccess);
                viewModel.TLSUsingWiFiResSuc = wifiUploadSessionsTLS.Average(u => u.AverageRespondToSuccess);
            }

            if (stationary3GUploadSessionsQUIC.Any() && stationary3GUploadSessionsTLS.Any())
            {
                viewModel.QuicUsing3GStationaryNewRip = stationary3GUploadSessionsQUIC.Average(u => u.AverageNewSessionToRequestIssuance);
                viewModel.TLSUsing3GStationaryNewRip = stationary3GUploadSessionsTLS.Average(u => u.AverageNewSessionToRequestIssuance);
                viewModel.QuicUsing3GStationaryResSuc = stationary3GUploadSessionsQUIC.Average(u => u.AverageRespondToSuccess);
                viewModel.TLSUsing3GStationaryResSuc = stationary3GUploadSessionsTLS.Average(u => u.AverageRespondToSuccess);
            }

            if (moving4GUploadSessionsQUIC.Any() && moving4GUploadSessionsTLS.Any())
            {
                viewModel.QuicUsing4GMovingNewRip = moving4GUploadSessionsQUIC.Average(u => u.AverageNewSessionToRequestIssuance);
                viewModel.TLSUsing4GMovingNewRip = moving4GUploadSessionsTLS.Average(u => u.AverageNewSessionToRequestIssuance);
                viewModel.QuicUsing4GMovingResSuc = moving4GUploadSessionsQUIC.Average(u => u.AverageRespondToSuccess);
                viewModel.TLSUsing4GMovingResSuc = moving4GUploadSessionsTLS.Average(u => u.AverageRespondToSuccess);
            }

            return viewModel;
        }
        

        #endregion
    }

}
