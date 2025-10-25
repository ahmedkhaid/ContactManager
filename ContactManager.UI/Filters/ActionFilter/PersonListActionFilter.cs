using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;
namespace CRUDExample.Filters.ActionFilter
{
    public class PersonListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonListActionFilter> _logger;
        public PersonListActionFilter(ILogger<PersonListActionFilter> logger)
        {
            _logger = logger;
        }
        //assign the view data to the view
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName}",nameof(PersonListActionFilter),nameof(OnActionExecuted));
            PersonsController personController = (PersonsController)context.Controller;
            Dictionary<string, object>? parameters = (Dictionary<string, object>?)context.HttpContext.Items["Arguments"];

            if (parameters != null)
            {
                if (parameters.ContainsKey("searchString"))
                {
                    personController.ViewBag.CurrentSerachString=parameters["searchString"];
                }
                if (parameters.ContainsKey("searchBy"))
                {
                    personController.ViewBag.CurrentSearchBy=parameters["searchBy"];   

                }
                if(parameters.ContainsKey("sortBy"))
                {
                    personController.ViewBag.sortBy=parameters["sortBy"];
                }
                
            }
            Dictionary<string, string> FilterOptions = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName),"Person name"},
                { nameof(PersonResponse.Gender),"Gender"},
                 { nameof(PersonResponse.Email),"Email"},
                  { nameof(PersonResponse.Address),"Address"},
                   { nameof(PersonResponse.Age),"Age"},
                   { nameof(PersonResponse.Country),"Country"},
                   { nameof(PersonResponse.DateOfBirth),"Date of birth"},
            };
            personController.ViewBag.SearchFields = FilterOptions;
        }
        //setting the sortBy as PersonName if itnot the sortby not equal one of the Person attribute
        
        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items["Arguments"]=context.ActionArguments;
            _logger.LogInformation("{FilterName}.{MethodName}",nameof(PersonListActionFilter),nameof(OnActionExecuting));

            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                context.HttpContext.Items["CurrentSearchBy"]=context.ActionArguments["searchBy"];

                _logger.LogInformation($"the seach by before updating {context.ActionArguments["searchBy"]}");
                var searchByOptions = new List<string>(){nameof(PersonResponse.PersonName),
                nameof(PersonResponse.Gender),nameof(PersonResponse.Address),
                nameof(PersonResponse.DateOfBirth),nameof(PersonResponse.Email),
                nameof(PersonResponse.Country)};

                if (searchByOptions.Any(temp => temp==Convert.ToString(context.ActionArguments["searchBy"]))==false)
                {
                    context.ActionArguments["searchBy"]=nameof(PersonResponse.PersonName);
                }
                _logger.LogInformation($"the search by value after updating{context.ActionArguments["searchBy"]}");
            }
           
        }
    }
}
