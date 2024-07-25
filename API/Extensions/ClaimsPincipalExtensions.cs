using System.Security.Claims;

namespace API.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="ClaimsPrincipal"/> class to retrieve user information from claims.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Retrieves the username from the claims principal.
        /// </summary>
        /// <param name="user">The <see cref="ClaimsPrincipal"/> object representing the current user.</param>
        /// <returns>The username as a string, or null if the claim is not found.</returns>
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        /// <summary>
        /// Retrieves the user ID from the claims principal.
        /// </summary>
        /// <param name="user">The <see cref="ClaimsPrincipal"/> object representing the current user.</param>
        /// <returns>The user ID as a string, or null if the claim is not found.</returns>
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
