using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions;
public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        // Configure the authentication service
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            // Configure the JWT token validation parameters
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // Indicates that the issuer signing key should be validated to ensure token validity.
                ValidateIssuerSigningKey = true,
                // Sets the issuer signing key obtained from the application's configuration.
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                // Disables validation of the token issuer.
                ValidateIssuer = false,
                // Disables validation of the token audience.
                ValidateAudience = false
            };
        });
        return services;
    }
}
