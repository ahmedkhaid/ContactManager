using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilter.ResourceFilter
{
    public class FeatureDisabledResource : IResourceFilter
    {
        private readonly bool _disabled;
        private readonly ILogger<FeatureDisabledResource> _logger;
        public FeatureDisabledResource( ILogger<FeatureDisabledResource> logger, bool IsDisabled = true)
        {
            _disabled = IsDisabled;
            _logger = logger;
        }
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            _logger.LogInformation("the end of the execution of the Feature Disabled filter");

        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            _logger.LogInformation("here is the start of the execution of {FilterName}", nameof(FeatureDisabledResource));
            if (_disabled)
            {
                context.Result = new StatusCodeResult(501);
            }
        }
    }
}
