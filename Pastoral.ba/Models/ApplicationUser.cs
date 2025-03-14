using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace Pastoral.ba.Models
{
    public class ApplicationUser : IdentityUser
    {

        [StringLength(13, MinimumLength = 13, ErrorMessage = "JMBG mora imati točno 13 brojeva.")]
        public string? JMBG { get; set; }

        [StringLength(11, MinimumLength = 11, ErrorMessage = "OIB mora imati točno 11 brojeva.")]
        public string? OIB { get; set; }

        [Required]
        public string PunoIme { get; set; }


    }
}