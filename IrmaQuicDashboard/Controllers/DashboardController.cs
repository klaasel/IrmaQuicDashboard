using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var viewModel = new DashboardViewModel();
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
        public IActionResult Dashboard(Guid uploadSessionMetadataId)
        {
            var irmaSessions = "test";
            return PartialView("_DashboardPartial");
        }
    }
}
