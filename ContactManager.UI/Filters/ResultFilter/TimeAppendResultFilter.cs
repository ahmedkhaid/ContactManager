using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResultFilter
{
    public class TimeAppendResultFilter:ResultFilterAttribute
    {
       
        public virtual void OnResultExecuted(ResultExecutedContext context)
        {
        }
        public virtual void OnResultExecuting(ResultExecutingContext context)
        {
                //context.HttpContext.Response.Headers["DateModified", DateTime.Now.ToString("yy-mm--dd")];
   
        }
    }
}
