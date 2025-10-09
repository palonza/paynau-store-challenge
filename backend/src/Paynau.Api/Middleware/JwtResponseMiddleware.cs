using Microsoft.AspNetCore.Http;
using Paynau.Infrastructure.Security;
using System.Threading.Tasks;

namespace Paynau.Api.Middleware
{
    public class JwtResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IJwtTokenService tokenService)
        {
            // Generar un nuevo token por cada request
            var token = tokenService.GenerateToken();
            context.Response.OnStarting(() =>
            {
                context.Response.Headers["X-Auth-Token"] = token;
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
