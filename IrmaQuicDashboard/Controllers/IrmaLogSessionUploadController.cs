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

namespace IrmaQuicDashboard.Controllers
{
    public class IrmaLogSessionUploadController : Controller
    {
        private readonly IAppLogEntriesRepository _appLogRepository;
        private readonly IServerLogEntriesRepository _serverLogRepository;

        public IrmaLogSessionUploadController(IAppLogEntriesRepository appLogRepository, IServerLogEntriesRepository serverLogRepository)
        {
            _appLogRepository = appLogRepository;
            _serverLogRepository = serverLogRepository;
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

            var uploadMetadata = _appLogRepository.CreateNewUploadSession(
                model.Date,
                model.Location,
                model.Description,
                model.UsesQuic,
                model.SessionNumberUploaded,
                model.AppLog
               
                );

            _serverLogRepository.ProcessServerLog(model.ServerLog, uploadMetadata.Id);

            return RedirectToAction("Index");
        }

        
    }
}
