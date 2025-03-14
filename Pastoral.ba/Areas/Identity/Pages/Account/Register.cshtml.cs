// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Pastoral.ba.Models;

namespace Pastoral.ba.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "GlobalAdmin")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public IList<IdentityRole> Roles { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Odaberi BK.")]
            [Display(Name = "Biskupska konferencija")]
            public string BK { get; set; }

            [StringLength(13, MinimumLength = 13, ErrorMessage = "JMBG mora imati točno 13 brojeva.")]
            [Display(Name = "JMBG")]
            public string? JMBG { get; set; }

            [StringLength(11, MinimumLength = 11, ErrorMessage = "OIB mora imati točno 11 brojeva.")]
            [Display(Name = "OIB")]
            public string? OIB { get; set; }

            [Required]
            [Display(Name = "Ime i prezime")]
            public string PunoIme { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Lozinka mora biti barem {2} i maksimalno {1} znakova.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Lozinka")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Potvrdi lozinku")]
            [Compare("Password", ErrorMessage = "Lozinke se ne podudaraju.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Uloga")]
            public string Role { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            Roles = _roleManager.Roles.ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Validacija: BiH korisnici moraju unijeti JMBG, a RH korisnici OIB
                if (Input.BK == "BKBiH" && string.IsNullOrWhiteSpace(Input.JMBG))
                {
                    ModelState.AddModelError("Input.JMBG", "JMBG je obavezan za korisnike BKBiH.");
                    return Page();
                }
                if (Input.BK == "HBK" && string.IsNullOrWhiteSpace(Input.OIB))
                {
                    ModelState.AddModelError("Input.OIB", "OIB je obavezan za korisnike HBK.");
                    return Page();
                }

                var user = CreateUser();
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                user.PunoIme = Input.PunoIme;
                user.JMBG = Input.BK == "BKBiH" ? Input.JMBG : null;
                user.OIB = Input.BK == "HBK" ? Input.OIB : null;

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Korisnik je kreirao novi račun.");

                    if (!await _roleManager.RoleExistsAsync(Input.Role))
                    {
                        var roleResult = await _roleManager.CreateAsync(new IdentityRole(Input.Role));
                        if (!roleResult.Succeeded)
                        {
                            foreach (var error in roleResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return Page();
                        }
                    }

                    await _userManager.AddToRoleAsync(user, Input.Role);

                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            Roles = _roleManager.Roles.ToList();
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Ne mogu stvoriti instancu '{nameof(ApplicationUser)}'.");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("Ova verzija UI-ja zahtijeva korisničku trgovinu s podrškom za e-mail.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
