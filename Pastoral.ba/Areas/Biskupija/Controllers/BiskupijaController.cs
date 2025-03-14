using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pastoral.ba.Data;

namespace Pastoral.ba.Areas.Biskupija.Controllers
{
    [Area("DioceseAdmin")]
    [Authorize(Roles = "DioceseAdmin")]
    public class BiskupijaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BiskupijaController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var dioceses = _context.Biskupije.Include(d => d.Zupe).ToList();
            return View(dioceses);
        }

        public IActionResult ManageParishes(int dioceseId)
        {
            var parishes = _context.Zupe
                .Where(p => p.ID_Biskupija == dioceseId)
                .Include(p => p.Zupnik)
                .ToList();
            return View(parishes);
        }
    }
}
