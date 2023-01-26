using Identity.Models;
using Identity.Seeds;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Extensions
{
    public static class IdentityExtensions
    {
        public async static void MigrateIdentity(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            
            try
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await DefaultRoles.SeedAsync(userManager, roleManager);
                await DefaultAdminUser.SeedAsync(userManager, roleManager);
                await DefaultBasicUser.SeedAsync(userManager, roleManager);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
