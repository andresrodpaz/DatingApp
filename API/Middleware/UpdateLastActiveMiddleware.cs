﻿using System;
using System.Threading.Tasks;
using API.Interfaces;
using Microsoft.AspNetCore.Http;

namespace API.Middleware
{
    public class UpdateLastActiveMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUnitOfWork _uow;

        public UpdateLastActiveMiddleware(RequestDelegate next, IUnitOfWork uow)
        {
            _next = next;
            _uow = uow;
        }

        public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
        {
            // Ejecutar la siguiente solicitud (middleware o controlador)
            await _next(context);

            // Verificar si el usuario está autenticado
            if (context.User.Identity.IsAuthenticated)
            {
                var username = context.User.Identity.Name;
                var user = await userRepository.GetUserByUsernameAsync(username);

                if (user != null)
                {
                    user.LastActive = DateTime.UtcNow;
                    await _uow.Complete();
                }
            }
        }
    }
}
