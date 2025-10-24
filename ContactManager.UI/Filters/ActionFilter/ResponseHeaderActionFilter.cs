using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilter
{
    public class ResponseHeaderActionFilter :IAsyncActionFilter
    {
        private readonly ILogger<ResponseHeaderActionFilter> _logger;
        private readonly string _key;
        private readonly string _value;
        public ResponseHeaderActionFilter(string key,string value,ILogger<ResponseHeaderActionFilter>logger)
        {
            _logger = logger;
            _key=key; 
            _value=value;
        }
        //public void OnActionExecuted(ActionExecutedContext context)
        //{
        //    _logger.LogInformation("method {FilterName} HeaderKey : {Key} HeaderValue : {Value}",nameof(ResponseHeaderActionFilter), Key, Value);
        //}

        //public void OnActionExecuting(ActionExecutingContext context)
        //{
        //    _logger.LogInformation("method {FilterName} HeaderKey : {Key} HeaderValue : {Value}", nameof(ResponseHeaderActionFilter), Key, Value);
        //    context.HttpContext.Response.Headers[Key]=Value;
        //}

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //adding the before logic
            _logger.LogInformation("method {FilterName} HeaderKey : {Key} HeaderValue : {Value} From Before Logic", nameof(ResponseHeaderActionFilter), _key, _value);
            await next();
            //adding  the after logic
            _logger.LogInformation("method {FilterName} HeaderKey : {Key} HeaderValue : {Value}", nameof(ResponseHeaderActionFilter), _key, _value);
            context.HttpContext.Response.Headers[_key]=_value;

        }
    }
}
