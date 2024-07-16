using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions;
public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {

        services.AddIdentityCore<AppUser>(opt => {
            opt.Password.RequireNonAlphanumeric = false;
        })
          .AddRoles<AppRole>()
          .AddRoleManager<RoleManager<AppRole>>()
          .AddEntityFrameworkStores<DataContext>();

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

            options.Events = new JwtBearerEvents{
                OnMessageReceived = context => {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs")){
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization(opt =>
        
        {
            opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin","Moderator"));
        });



        return services;
    }
}
