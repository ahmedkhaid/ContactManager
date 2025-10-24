using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ExceptionFilter
{
    public class ExceptionHandleFilter:IExceptionFilter
    {
        private readonly IWebHostEnvironment _host;
        private readonly ILogger<ExceptionHandleFilter> _logger;
        public ExceptionHandleFilter(IWebHostEnvironment hosting,ILogger<ExceptionHandleFilter>logger)
        {
            _host = hosting;
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError("The exception type is {exception} Message : {}", context.Exception.GetType().ToString(), context.Exception.Message);
            if(_host.IsDevelopment())
            {
                context.Result=new ContentResult() { Content=context.Exception.Message,StatusCode=500};

            }
        }
    }
}
