using Microsoft.AspNetCore.Builder;

namespace WebApplication.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseNhibernateMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NhibernateMiddleware>();
        }
    }
}