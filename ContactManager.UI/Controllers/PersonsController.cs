using CRUDExample.Filters.ActionFilter;
using CRUDExample.Filters.ActionFilter.AuthorizationFilter;
using CRUDExample.Filters.ActionFilter.ResourceFilter;
using CRUDExample.Filters.ResultFilter;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using CustomException;
namespace CRUDExample.Controllers
{
    public class PersonsController : Controller
    {
        private readonly ILogger<PersonsController> _logger;
        private readonly IPersonsGetterService _personGetterService;
        private readonly IPersonsAddService _personAddService;
        private readonly IPersonsDeleteService _personsDeleteService;
        private readonly IPersonUpdateService _personUpdateService;
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsSortService _personsSortService;
        private readonly ICountriesGetterService _countriesGetterService;
  

        public PersonsController(IPersonsGetterService personGetterService, ICountriesGetterService countriesService,ILogger<PersonsController> logger,IPersonsDeleteService personsDeleteService, IPersonsAddService personsAddService, IPersonsSortService personsSortService , IPersonUpdateService personUpdateService)
        {
            _personGetterService=personGetterService;
            _countriesGetterService=countriesService;
            _logger=logger;
            _personAddService = personsAddService;
            _personsSortService=personsSortService;
            _personUpdateService=personUpdateService;
           _personsDeleteService = personsDeleteService;
           

        }
        [Route("Persons/index")]
        [Route("/")]
        [TypeFilter(typeof(ResponseHeaderActionFilter),Arguments =new object[]{"X-Custom_key" ,"X-Custom-Value"})]
        [TypeFilter(typeof(PersonListActionFilter))]
        [TimeAppendResultFilter]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), OrderOptions sortOrder = OrderOptions.ASC)
        {
            _logger.LogDebug($"SearchBy is {searchBy} and the search string {searchString}");
            _logger.LogInformation("index action method from the controller");
           
            List<PersonResponse> people =await _personGetterService.GetFilteredPersons(searchBy, searchString);
            //creating the sort functionality
            List<PersonResponse> sortesPeople =await _personsSortService.GetSortedPersons(people, sortBy, sortOrder);
            ViewBag.CurrentSortBy=sortBy;
            ViewBag.CurrentSortOrder=sortOrder.ToString();
            ViewBag.PageContent="Create Person";
            ViewBag.PageDirection="Create";
            return View(sortesPeople);

        }
        [Route("persons/create")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries =await _countriesGetterService.GetAllCountries();
            ViewBag.countriesID=countries.Select(temp => new SelectListItem { Text=temp.CountryName, Value=temp.CountryID.ToString() });
            ViewBag.countriesList=countries;
            return View();
        }
        [Route("~/Persons/Create")]
        [HttpPost]
        [TypeFilter(typeof(PersonCreateAndEditActionFilter))]
        [TypeFilter(typeof(FeatureDisabledResource))]
        public async Task<IActionResult> Create(PersonAddRequest? personRequest)
        {

          
           PersonResponse response =await _personAddService.AddPerson(personRequest);
            return RedirectToAction("Index", "Persons");
        }
        //edit persons
        [HttpGet("Persons/Edit/{personID}")]

        [TypeFilter(typeof(TokenAppendFilter))]
        public async Task<IActionResult> Edit(Guid? personID)
        {
            List<CountryResponse> countriesList = await _countriesGetterService.GetAllCountries();
            ViewBag.countriesID=countriesList.Select(c => new SelectListItem { Text=c.CountryName, Value=c.CountryID.ToString() });
            PersonResponse? person = await _personGetterService.GetPersonById(personID);
            PersonUpdateRequest? personUpdate = person.ToPersonUpdateRequest();
            ViewBag.person=personUpdate;
            return View(personUpdate);
        }
        [HttpPost("Persons/Edit/{personID}")]
        [TypeFilter(typeof(PersonCreateAndEditActionFilter))]
       
        //[TypeFilter(typeof(TokenAuthorizationFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest ?personRequest)
        {
            PersonResponse ?response =await _personGetterService.GetPersonById(personRequest?.PersonId);
            if(response==null)
            {
                return RedirectToAction("index");
            }
            //if(personRequest!=null)
            //personRequest.PersonId=Guid.NewGuid();
            
        PersonResponse updatePerson =  await _personUpdateService.UpdatePerson(personRequest);
          
            return RedirectToAction("index");
        }
        [HttpGet("Persons/Delete/{PersonID}")]
        public async Task<IActionResult> Delete(Guid PersonID) {
            PersonResponse ?response = await _personGetterService.GetPersonById(PersonID);
            if(response==null)
            {
                return RedirectToAction("index");
            }
            return View(response);
        }
        [HttpPost("Persons/Delete/{PersonID}")]
        public async Task<IActionResult> Delete(Guid?PersonID)
        {
            PersonResponse? response = await _personGetterService.GetPersonById(PersonID);
            if (response==null)
            {
                return RedirectToAction("index");
            }
            bool IsDeleted = await _personsDeleteService.DeletePerson(PersonID);
            return RedirectToAction("index");
        }
        [Route("Persons/PersonPdf")]
        public async Task<IActionResult>PersonPdf()
        {
            List<PersonResponse> People =await _personGetterService.GetAllPerson(); 
            return new ViewAsPdf("PersonPdf", People, ViewData);
        }
        [Route("Persons/PersonCsv")]
        public async Task<IActionResult> PersonCsv()
        {
            MemoryStream csvMemory = await _personGetterService.GetCsv();
            return File(csvMemory, "application/octet-stream", "Person.csv");
        }
        [Route("Persons/PersonExcel")]
        public async Task<IActionResult> PersonExcel()
        {
            MemoryStream csvMemory = await _personGetterService.GetPeopleExcel();
            return File(csvMemory, " application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Person.xlsx");
        }

    }
}
