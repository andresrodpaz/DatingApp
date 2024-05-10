using API.Data;
using API.Services;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddAplicationServices(this IServiceCollection services, IConfiguration config)
    {
        // Registering a DbContext in the dependency injection service.
        services.AddDbContext<DataContext>(opt =>
        {
            // Configuring the DbContext to use SQLite as the database provider.
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });

        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        return services;

    }
}
