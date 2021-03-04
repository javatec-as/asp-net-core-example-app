using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApplication.Nhibernate;

namespace WebApplication.Middleware
{
    public class NhibernateMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISessionManager _sessionManager;

        public NhibernateMiddleware(RequestDelegate next, ISessionManager sessionManager)
        {
            _next = next;
            _sessionManager = sessionManager;
        }

        // ReSharper disable once UnusedMember.Global
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception)
            {
                // handle any errors from further down the pipeline
                await _sessionManager.FinalizeSessionAsync(true);
                throw; // pass exception to next middleware component
            }

            // if we get here, next is commit which might throw an exception
            await _sessionManager.FinalizeSessionAsync();
        }
    }
}