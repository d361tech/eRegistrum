using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pastoral.ba.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pastoral.ba.Areas.Admin.Controllers
{
    [Authorize(Roles = "GlobalAdmin")]
    [Area("Admin")] // ⬅️ Ovo osigurava da je u "Admin" Area
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserManager<ApplicationUser> userManager, ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        // 📌 1. Pregled svih korisnika
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            return View(users); // Provjeri da li postoji /Areas/Admin/Views/Users/Index.cshtml
        }

        // 📌 2. Uređivanje korisnika
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("ID nije proslijeđen.");
                return BadRequest("ID nije proslijeđen.");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning($"Korisnik s ID-om {id} nije pronađen.");
                return NotFound("Korisnik nije pronađen.");
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState nije validan.");
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                _logger.LogWarning($"Korisnik s ID-om {model.Id} nije pronađen.");
                return NotFound("Korisnik nije pronađen.");
            }

            user.PunoIme = model.PunoIme;
            user.JMBG = model.JMBG;
            user.OIB = model.OIB;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError("Greška prilikom ažuriranja korisnika.");
                ModelState.AddModelError("", "Greška prilikom ažuriranja korisnika.");
                return View(model);
            }

            return RedirectToAction("Index");
        }

        // 📌 3. Brisanje korisnika
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("ID nije proslijeđen za brisanje.");
                return BadRequest("ID nije proslijeđen.");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning($"Korisnik s ID-om {id} nije pronađen.");
                return NotFound("Korisnik nije pronađen.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError("Greška prilikom brisanja korisnika.");
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}
