using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ZGym.Core.Entities;

namespace ZGym.Data.Data
{
    public class SeedData
    {
        public static async Task InitAsync(IServiceProvider services, String AdminPW)
        {
            using (var context = new ApplicationDbContext(services.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // if (await context.GymClasses.AnyAsync())
                // {
                //     return;
                // }

                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                var fake = new Faker("sv");

                var gymClasses = new List<GymClass>();

                for (var i = 0; i < 20; i++)
                {
                    var gymClass = new GymClass
                    {
                        Name = fake.Company.CatchPhrase(),
                        Description = fake.Hacker.Verb(),
                        Duration = new TimeSpan(0, 55, 0),
                        StartTime = fake.Date.Soon(16).AddDays(-2)
                        // StartTime = DateTime.Now.AddDays(fake.Random.Int(-2, 2))
                    };

                    gymClasses.Add(gymClass);
                }

                await context.AddRangeAsync(gymClasses);

                var roleNames = new[] { "Admin", "Member" };

                foreach (var roleName in roleNames)
                {
                    if (await roleManager.RoleExistsAsync(roleName))
                    {
                        continue;
                    }

                    var role = new IdentityRole { Name = roleName };
                    var result = await roleManager.CreateAsync(role);

                    if (!result.Succeeded)
                    {
                        throw new Exception(string.Join("\n", result.Errors));
                    }
                }

                var adminEmail = "admin@gym.se";
                var foundAdmin = await userManager.FindByEmailAsync(adminEmail);

                if (foundAdmin != null)
                {
                    return;
                }

                var admin = new ApplicationUser
                {
                    UserName = "Admin",
                    FirstName = "Admin",
                    LastName = "A",
                    Email = adminEmail
                };

                var addAdminResult = await userManager.CreateAsync(admin, AdminPW);

                if (!addAdminResult.Succeeded)
                {
                    throw new Exception(string.Join("\n", addAdminResult.Errors));
                }

                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                foreach (var role in roleNames)
                {
                    if (await userManager.IsInRoleAsync(adminUser, role))
                    {
                        continue;
                    }
                    var addToRoleResult = await userManager.AddToRoleAsync(adminUser, role);

                    if (!addToRoleResult.Succeeded)
                    {
                        throw new Exception(string.Join("\n", addToRoleResult.Errors));
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}