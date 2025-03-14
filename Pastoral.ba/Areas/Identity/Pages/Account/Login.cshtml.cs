// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Pastoral.ba.Models;
using Microsoft.EntityFrameworkCore;

namespace Pastoral.ba.Areas.Identity.Pages.Account
{
    public class LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<LoginModel> logger) : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<LoginModel> _logger = logger;

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "JMBG ili OIB je obavezan.")]
            [StringLength(13, MinimumLength = 11, ErrorMessage = "JMBG mora imati 13 brojeva ili OIB 11 brojeva.")]
            [Display(Name = "JMBG ili OIB")]
            public string JMBGorOIB { get; set; } // Može sadržavati JMBG ili OIB

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Lozinka")]
            public string Password { get; set; }

            [Display(Name = "Zapamti me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Dohvati korisnika prema JMBG-u ili OIB-u
                var user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.JMBG == Input.JMBGorOIB || u.OIB == Input.JMBGorOIB);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Neispravan JMBG, OIB ili lozinka.");
                    return Page();
                }

                var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Korisnik prijavljen.");

                    // Dohvati uloge korisnika
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("GlobalAdmin"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    else if (roles.Contains("Biskupija"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Biskupija" });
                    }
                    else if (roles.Contains("Zupa"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Zupa" });
                    }

                    return LocalRedirect(returnUrl); // Ako nema određenu ulogu, vrati ga na početnu
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Korisnički nalog zaključan.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Neispravan JMBG, OIB ili lozinka.");
                    return Page();
                }
            }
            return Page();
        }
    }
}
