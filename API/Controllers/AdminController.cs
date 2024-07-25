using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    /// <summary>
    /// Provides endpoints for user administration tasks, including role management, user blocking, and verification.
    /// </summary>
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminController"/> class.
        /// </summary>
        /// <param name="userManager">The user manager service for user operations.</param>
        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Retrieves a list of users along with their assigned roles.
        /// Requires the user to be authorized with the "RequireAdminRole" policy.
        /// </summary>
        /// <returns>A list of users with their roles.</returns>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    Username = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);
        }

        /// <summary>
        /// Edits the roles of a user specified by the username.
        /// Requires the user to be authorized with the "RequireAdminRole" policy.
        /// </summary>
        /// <param name="username">The username of the user whose roles are to be edited.</param>
        /// <param name="roles">Comma-separated string of roles to be assigned to the user.</param>
        /// <returns>The updated list of roles for the user.</returns>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");
            var selectedRoles = roles.Split(',').ToArray();

            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return NotFound();
            var userRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded) return BadRequest("Failed to add roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        /// <summary>
        /// Retrieves a list of photos that require moderation.
        /// Requires the user to be authorized with the "ModeratePhotoRole" policy.
        /// </summary>
        /// <returns>A message indicating access to photos for moderation.</returns>
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Admins or moderators can see this");
        }

        /// <summary>
        /// Blocks a user specified by the username.
        /// Requires the user to be authorized with the "RequireAdminRole" policy.
        /// </summary>
        /// <param name="username">The username of the user to be blocked.</param>
        /// <returns>A message indicating whether the user was blocked successfully.</returns>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("block-user/{username}")]
        public async Task<ActionResult> BlockUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound();

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue; // Block indefinitely
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest("Failed to block the user");

            return Ok("User blocked successfully");
        }

        /// <summary>
        /// Unblocks a user specified by the username.
        /// Requires the user to be authorized with the "RequireAdminRole" policy.
        /// </summary>
        /// <param name="username">The username of the user to be unblocked.</param>
        /// <returns>A message indicating whether the user was unblocked successfully.</returns>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("unblock-user/{username}")]
        public async Task<ActionResult> UnblockUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound();

            user.LockoutEnd = null; // Unblock the user
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest("Failed to unblock the user");

            return Ok("User unblocked successfully");
        }

        /// <summary>
        /// Deletes a user specified by the username.
        /// Requires the user to be authorized with the "RequireAdminRole" policy.
        /// </summary>
        /// <param name="username">The username of the user to be deleted.</param>
        /// <returns>A message indicating whether the user was deleted successfully.</returns>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("delete-user/{username}")]
        public async Task<ActionResult> DeleteUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded) return BadRequest("Failed to delete the user");

            return Ok("User deleted successfully");
        }

        /// <summary>
        /// Verifies a user's email address by setting the email confirmation status to true.
        /// Requires the user to be authorized with the "RequireAdminRole" policy.
        /// </summary>
        /// <param name="username">The username of the user to be verified.</param>
        /// <returns>A message indicating whether the user was verified successfully.</returns>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("verify-user/{username}")]
        public async Task<ActionResult> VerifyUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound();

            user.EmailConfirmed = true; // Assuming email verification for the example
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest("Failed to verify the user");

            return Ok("User verified successfully");
        }
    }
}
