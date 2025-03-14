using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pastoral.ba.Models;

namespace Pastoral.ba.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Glavna stranica koja prikazuje informacije o prijavljenom korisniku
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Provjera da li je korisnik prijavljen
            if (!User.Identity.IsAuthenticated)
            {
                // Automatsko preusmjeravanje na LoginPath iz konfiguracije kolaèiæa
                return Challenge();
            }

            // Dohvatanje korisnika iz UserManager-a
            var user = await _userManager.GetUserAsync(User);

            // Provjera da li je korisnik pronaðen
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Prosljeðivanje korisnika u View
            return View(user);
        }

        // Privacy stranica
        public IActionResult Privacy()
        {
            return View();
        }

        // Error stranica
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
