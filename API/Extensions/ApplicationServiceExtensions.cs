using API.Data;
using API.Services;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using API.Helpers;

namespace API.Extensions;
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddAplicationServices(this IServiceCollection services, IConfiguration config)
    {
        // Registering a DbContext in the dependency injection service.
        services.AddDbContext<DataContext>(opt =>
        {
            // Configuring the DbContext to use SQLite as the database provider.
            opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        });

        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();

        

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));  
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<LogUserActivity>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();


        services.AddSignalR();
        services.AddSingleton<PresenceTracker>();

        return services;

    }
}
