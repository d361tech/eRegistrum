using Microsoft.AspNetCore.Identity;

namespace Pastoral.ba.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Definišite role koje želite dodati
            string[] roles = { "GlobalAdmin", "GenVikar", "Zupnik", "Kapelan", "Zupljanin", "Ekonom" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
