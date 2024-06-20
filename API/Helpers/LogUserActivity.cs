using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers;
public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
{
    var resultContext = await next();

    // Verificar si el usuario está autenticado
    if (!resultContext.HttpContext.User.Identity.IsAuthenticated)
    {
        return;
    }

    // Obtener el ID del usuario
    var userId = resultContext.HttpContext.User.GetUserId();

    // Obtener el repositorio de usuarios desde los servicios
    var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

    // Obtener el usuario por su ID
    //var user = await repo.GetUserByIdAsync(int.Parse(userId));
    var user = await repo.GetUserByIdAsync(int.Parse(userId));
    
        // Actualizar la propiedad LastActive
        user.LastActive = DateTime.UtcNow;
        await repo.SaveAllAsync();
    
}

    
}
