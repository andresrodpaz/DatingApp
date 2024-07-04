using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;
public class TokenService : ITokenService
{
     // SymmetricSecurityKey to store the secret key for token generation
    private readonly SymmetricSecurityKey _key;
    private readonly UserManager<AppUser> _userManager;

    /// <summary>
    /// Initializes a new instance of the TokenService class with the provided configuration.
    /// </summary>
    /// <param name="config">Configuration object containing the token key.</param>
    public TokenService(IConfiguration config, UserManager<AppUser> userManager)
    {
        // Convert the TokenKey from configuration into a byte array and assign it to _key
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        _userManager = userManager;
    }
    /// <summary>
    /// Creates a JWT token for the specified AppUser.
    /// </summary>
    /// <param name="user">The AppUser for whom the token is created.</param>
    /// <returns>Generated JWT token as a string.</returns>
    public async Task<string> CreateToken(AppUser user)
    {
        // Define a list of claims for the token, including the user's NameId (username)
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
        };

        var roles = await _userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Create SigningCredentials using the SymmetricSecurityKey and HmacSha512Signature algorithm
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        // Set up a SecurityTokenDescriptor with token details such as claims, expiration, and signing credentials
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),              // Set subject with the list of claims
            Expires = DateTime.Now.AddDays(7),                // Set token expiration to 7 days from now
            SigningCredentials = creds                         // Set the signing credentials for token validation
        };

        // Create a JwtSecurityTokenHandler to generate the JWT token
        var tokenHandler = new JwtSecurityTokenHandler();
        
        // Create the JWT token using the SecurityTokenDescriptor
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Write the token into a string representation
        return tokenHandler.WriteToken(token);
    }

}
