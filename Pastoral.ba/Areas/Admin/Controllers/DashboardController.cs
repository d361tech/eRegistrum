using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pastoral.ba.Areas.Admin;
using Pastoral.ba.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pastoral.ba.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "GlobalAdmin")]
    public class DashboardController : Controller
    {
        public DashboardController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

    }
}
