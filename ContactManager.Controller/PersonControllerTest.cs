using AutoFixture;
using Castle.Core.Logging;
using CRUDExample;
using CRUDExample.Controllers;
using Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace TestProject2
{
   
    public class PersonControllerTest

    {
        private readonly PersonsController _personsController;
        //creating filter for the PersonService
        private readonly IPersonsGetterService _personGetterService;
        private readonly IPersonsAddService _personAddService;
        private readonly IPersonsDeleteService _personDeleteService;
        private readonly IPersonUpdateService _personUpdateService;
        private readonly IPersonsSortService _personSortService;

        //mocking service
        private readonly Mock<IPersonsGetterService> _personGetterServiceMock;
        private readonly Mock<IPersonsAddService> _personAddServiceMock;
        private readonly Mock<IPersonsDeleteService> _personDeleteServiceMock;
        private readonly Mock<IPersonUpdateService> _personUpdateServiceMock;
        private readonly Mock<IPersonsSortService> _personSortServiceMock;
        ///
        private readonly Mock<ICountriesGetterService> _countryServiceMock;
      
        private readonly ICountriesGetterService _countriesService;
        private readonly IFixture _fixture;
        private readonly ILogger<PersonsController> _logger;
        private readonly Mock<ILogger<PersonsController>> _mockLogger;
        public PersonControllerTest()
        {
            _mockLogger = new Mock<ILogger<PersonsController>>();
            _logger=_mockLogger.Object;
            _fixture = new Fixture();
            //instanciating _personServicesMocks 
            _personAddServiceMock=new();
            _personDeleteServiceMock=new();
            _personUpdateServiceMock=new();
            _personSortServiceMock=new();
            _personGetterServiceMock= new Mock<IPersonsGetterService>();
            //mocking the personService
            _personGetterService = _personGetterServiceMock.Object;
            _personAddService = _personAddServiceMock.Object;
            _personDeleteService = _personDeleteServiceMock.Object;
            _personUpdateService = _personUpdateServiceMock.Object;
            _personSortService = _personSortServiceMock.Object;
            //mocking coutry service and Instanciating Person Controller object
            _countryServiceMock =new Mock<ICountriesGetterService> ();
            _countriesService = _countryServiceMock.Object;
            /*
             * IPersonsGetterService personGetterService, ICountriesService countriesService,ILogger<PersonsController> logger,IPersonsDeleteService personsDeleteService, IPersonsAddService personsAddService, IPersonsSortService personsSortService , IPersonUpdateService personUpdateService)
        {
             * */
            _personsController = new PersonsController(_personGetterService,_countriesService, _logger, _personDeleteService, _personAddService, _personSortService, _personUpdateService);
        }
        #region Index
        [Fact]
        public async Task Create_indexView_returns_View_with_personList()
        {

            //need to mock two method one the GetFilterMethod and GetSorted method
            List<Person> persons = new List<Person>() {_fixture.Build<Person>().With(p=>p.Country,null as Country).With(p=>p.Email,"example@hotmail.com").Create(),
            _fixture.Build<Person>().With(p=>p.Country,null as Country).With(p=>p.Email,"example@hotmail.com").Create(),
            _fixture.Build<Person>().With(p=>p.Country,null as Country).With(p=>p.Email,"example@hotmail.com").Create(),};
            List<PersonResponse> personList_expected = persons.Select(p => p.ToPersonRespose()).ToList();
            _personGetterServiceMock.Setup(p => p.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(personList_expected);
            _personSortServiceMock.Setup(p=>p.GetSortedPersons(It.IsAny<List<PersonResponse>>(),It.IsAny<string>(),It.IsAny<OrderOptions>())).ReturnsAsync(personList_expected);
            IActionResult result=await _personsController.Index(nameof(Person.PersonName), "sa");
           var viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().BeEquivalentTo(personList_expected);
        }
        #endregion
        #region Create
       
        [Fact]
        public async Task CreatePerson_Valid_PersonAddRequest_personResponse_redirectToIndexActionMethod()
        {
            //PersonAddRequest personRequest= _fixture.Build<PersonAddRequest>().Without(p=>p.PersonName).Create();
            PersonAddRequest personRequest = _fixture.Create<PersonAddRequest>();

            List<CountryResponse> countries = new() { _fixture.Build<CountryResponse>().Create(),
            _fixture.Build<CountryResponse>().Create(),
            _fixture.Build<CountryResponse>().Create()
            };
            PersonResponse personResponse = _fixture.Create<PersonResponse>();
           
            _personAddServiceMock.Setup(p => p.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(personResponse);
            IActionResult result = await _personsController.Create(personRequest);
            var redirectViewResult = Assert.IsType<RedirectToActionResult>(result);
            redirectViewResult.ActionName.Should().Be("Index");
            redirectViewResult.ControllerName.Should().Be("Persons");

        }


        #endregion

    }
}
