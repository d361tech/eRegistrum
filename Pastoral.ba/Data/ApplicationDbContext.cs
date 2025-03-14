using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pastoral.ba.Models;

namespace Pastoral.ba.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // Konstruktor koji prima opcije
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet za vaše modele
        public DbSet<Biskupija> Biskupije { get; set; }
        public DbSet<Zupa> Zupe { get; set; }
        public DbSet<Osoba> Osobe { get; set; }
    }
}
