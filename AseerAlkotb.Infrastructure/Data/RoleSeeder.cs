using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AseerAlkotb.Infrastructure.Data
{
   public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider services)
        {
            try
            {
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

                string[] roles = { "Admin", "Staff", "Client" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole<int>(role));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while seeding roles: {ex.Message}");
            }
        }
    }
}
