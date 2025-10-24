using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Threading.Tasks;

namespace CRUDExample.MiddleWare
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class UseCustomExceptionHandleMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UseCustomExceptionHandleMiddleWare> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public UseCustomExceptionHandleMiddleWare(RequestDelegate next, ILogger<UseCustomExceptionHandleMiddleWare> logger,IDiagnosticContext diagnosticContext)
        {
            _next = next;
            _logger=logger;
            _diagnosticContext=diagnosticContext;
        }

        public async Task Invoke(HttpContext httpContext)
        {

            try
            {
                await _next(httpContext);
            }
            catch (Exception ex) {
                if(ex.InnerException!=null)
                {
                    _logger.LogError("{ExceptionType} {ExceptionMessage}", ex.InnerException.GetType().ToString(), ex.Message);
                }
                else
                {
                    _logger.LogError("{ExceptionType} {ExceptionMessage}", ex.GetType().ToString(), ex.Message);

                }
                //adding custom exception
                //httpContext.Response.StatusCode=500;
                //await httpContext.Response.WriteAsync("An Error has Occured");
                throw;
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class UseCustomExceptionHandleMiddleWareExtensions
    {
        public static IApplicationBuilder UseUseCustomExceptionHandleMiddleWare(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UseCustomExceptionHandleMiddleWare>();
        }
    }
}
