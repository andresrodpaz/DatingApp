using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, ILogger logger)
        {
            if (await userManager.Users.AnyAsync())
            {
                logger.LogInformation("Users already exist in the database. Skipping seeding.");
                return;
            }

            var userDataPath = "Data/UserSeedData.json";
            if (!File.Exists(userDataPath))
            {
                logger.LogError($"The file {userDataPath} does not exist.");
                return;
            }

            var userData = await File.ReadAllTextAsync(userDataPath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var roles = new List<AppRole>{
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"}
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            List<AppUser> users;
            try
            {
                users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error deserializing user data: {ex.Message}");
                return;
            }

            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();
                var result = await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");

                if (result.Succeeded)
                {
                    logger.LogInformation($"User {user.UserName} created successfully.");
                }
                else
                {
                    logger.LogError($"Failed to create user {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            var admin = new AppUser
            {
                UserName = "admin",
            };

            var adminResult = await userManager.CreateAsync(admin, "Pa$$w0rd");
            if (adminResult.Succeeded)
            {
                var rolesToAdd = new[] { "Admin", "Moderator" };
                var addToRolesResult = await userManager.AddToRolesAsync(admin, rolesToAdd);

                if (addToRolesResult.Succeeded)
                {
                    logger.LogInformation($"Admin user created successfully and added to roles: {string.Join(", ", rolesToAdd)}");
                }
                else
                {
                    logger.LogError($"Failed to add admin user to roles: {string.Join(", ", addToRolesResult.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                logger.LogError($"Failed to create admin user: {string.Join(", ", adminResult.Errors.Select(e => e.Description))}");
            }
        }
    }
}
