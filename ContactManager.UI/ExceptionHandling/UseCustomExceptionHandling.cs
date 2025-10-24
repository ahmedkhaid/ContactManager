using Microsoft.AspNetCore.Mvc;

namespace CRUDExample.ExceptionHandling
{
    public class UseCustomExceptionHandling
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UseCustomExceptionHandling> _logger;
        public UseCustomExceptionHandling(RequestDelegate next, ILogger<UseCustomExceptionHandling> logger)
        {
            _next=next;
            _logger=logger;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) {
                if (ex.InnerException!=null)
                {
                    _logger.LogError("The Erro Message is {errorMessage} and the errro type {errorType}", ex.InnerException.Message, ex.InnerException.GetType());
                }
                else
                {
                    _logger.LogError("The Erro Message is {errorMessage} and the errro type {errorType}", ex.Message, ex.GetType());
                }
                await context.Response.WriteAsync("an error has been occurred");
            }
        }
    }
    public static class UseCustomExceptionMiddelWareExtenstion{
        public static IApplicationBuilder UseCustomExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UseCustomExceptionHandling>();
        }
    }
}
