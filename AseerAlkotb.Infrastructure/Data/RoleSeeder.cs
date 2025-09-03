using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Entites.Models;
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
                var userManager = services.GetRequiredService<UserManager<User>>();

                string[] roles = { "Admin", "Client" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole<int>(role));
                    }
                }
                Console.WriteLine("Roles seeded successfully.");

                var adminEmail = "admin@test.com";
                var admin = await userManager.FindByEmailAsync(adminEmail);
                if (admin == null)
                {
                    var newAdmin = new User
                    {
                        // username:admin
                        // Email:admin@test.com
                        // password:Admin@123

                        UserName = "admin",
                        Email = adminEmail,
                        EmailConfirmed = true,
                        FirstName = "System",
                        LastName = "Admin",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    var result = await userManager.CreateAsync(newAdmin, "Admin@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newAdmin, "Admin");
                        Console.WriteLine("Admin user created and assigned to Admin role.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    Console.WriteLine("Admin user already exists.");
                }

                // seed Client
                var clientEmail = "user@user.com";
                var client = await userManager.FindByEmailAsync(clientEmail);

                // username:user
                // Email:user@user.com
                // password:User@123
                if (client == null) {
                    var newUser= new User {
                        UserName = "user",
                        Email = clientEmail,
                        EmailConfirmed = true,
                        FirstName = "Default",
                        LastName = "Client",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    var result = await userManager.CreateAsync(newUser, "User@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newUser, "Client");
                        Console.WriteLine("Client user created and assigned to Client role.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to create client user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
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
