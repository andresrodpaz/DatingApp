using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Data
{
    /// <summary>
    /// Provides methods for seeding initial user and role data into the database.
    /// </summary>
    public class Seed
    {
        /// <summary>
        /// Seeds users and roles into the database if no users exist.
        /// </summary>
        /// <param name="userManager">The <see cref="UserManager{AppUser}"/> used to manage users.</param>
        /// <param name="roleManager">The <see cref="RoleManager{AppRole}"/> used to manage roles.</param>
        /// <param name="logger">The <see cref="ILogger"/> used for logging information and errors.</param>
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, ILogger logger)
        {
            // Check if any users already exist in the database.
            if (await userManager.Users.AnyAsync())
            {
                logger.LogInformation("Users already exist in the database. Skipping seeding.");
                return;
            }

            // Path to the user data JSON file.
            var userDataPath = "Data/UserSeedData.json";
            if (!File.Exists(userDataPath))
            {
                logger.LogError($"The file {userDataPath} does not exist.");
                return;
            }

            // Read the user data from the JSON file.
            var userData = await File.ReadAllTextAsync(userDataPath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Define roles to be created.
            var roles = new List<AppRole>
            {
                new AppRole { Name = "Member" },
                new AppRole { Name = "Admin" },
                new AppRole { Name = "Moderator" }
            };

            // Create roles in the database.
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            // Deserialize the user data from JSON.
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

            // Create users and assign the "Member" role.
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

            // Create an admin user with elevated permissions.
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
