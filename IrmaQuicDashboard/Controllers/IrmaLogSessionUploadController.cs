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
        private readonly ILogEntriesRepository _repository;

        public IrmaLogSessionUploadController(ILogEntriesRepository repository)
        {
            _repository = repository;
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

            _repository.CreateNewUploadSession(
                model.Date,
                model.Location,
                model.Description,
                model.UsesQuic,
                model.SessionNumberUploaded,
                model.AppLog,
                model.ServerLog
                );

            return RedirectToAction("Index");
        }

        
    }
}
