using System.Text;
using API;
using API.Data;
using API.Interfaces;
using API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using API.Middleware;
using API.Helpers;
using Microsoft.AspNetCore.Identity;
using API.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<ExceptionMiddleware>();
//app.UseMiddleware<UpdateLastActiveMiddleware>();


app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();


app.UseHttpsRedirection();
app.MapControllers();

// Seed the database
// using var scope = app.Services.CreateScope();
// var services = scope.ServiceProvider;
// try
// {
//     var context = services.GetRequiredService<DataContext>();
//     var userManager = services.GetRequiredService<UserManager<AppUser>>();
//     var logger = services.GetRequiredService<ILogger<Program>>();
//     var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
//     await context.Database.MigrateAsync();
//     await Seed.SeedUsers(userManager, roleManager,logger);
// }
// catch (Exception ex)
// {
//     var logger = services.GetRequiredService<ILogger<Program>>();
//     logger.LogError(ex, "An error occurred during migration");
// }

app.Run();


