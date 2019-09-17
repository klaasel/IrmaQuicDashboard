using System;
using Microsoft.AspNetCore.Mvc;

namespace IrmaQuicDashboard.Controllers
{
    public class IrmaSessionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
