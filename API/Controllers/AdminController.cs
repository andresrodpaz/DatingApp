using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
public class AdminController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;

    public AdminController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles(){
        var users = await _userManager.Users
        .OrderBy(u => u.UserName)
        .Select(u => new{
            u.Id,
            Username = u.UserName,
            Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
        })
        .ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles){
        if(string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");
        var selectedRoles = roles.Split(',').ToArray();

        var user = await _userManager.FindByNameAsync(username);

        if(user == null) return NotFound();
        var userRoles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if(!result.Succeeded) return BadRequest("Failed to add roles");

        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        if(!result.Succeeded) return BadRequest("Failed to remove from roles");
        return Ok(await _userManager.GetRolesAsync(user));

    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration(){
        return Ok("Admins or moderators can see this");
    }

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
