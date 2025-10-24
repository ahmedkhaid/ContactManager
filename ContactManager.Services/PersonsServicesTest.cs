using AutoFixture;
using Azure;
using Castle.Core.Logging;
using Entities;
using FluentAssertions;
using FluentAssertions.Execution;
using IRepositoryContract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CRUDTests
{
    public class PersonsServicesTest
    {
        //adding person InterFace for dependancy injection
        private readonly IPersonsAddService _personAddService;
        private readonly IPersonsDeleteService _personDeleteService;
        private readonly IPersonsGetterService _personGetterService;
        private readonly IPersonUpdateService _personUpdateService;
        private readonly IPersonsSortService _personSortService;
        //country Filed
        private readonly ICountriesGetterService _countriesGetterService;
        private readonly ICountriesAddService _countriesAddService;
        private readonly ICountriesUploadService _countriesUploadService;
        private readonly IPersonRepository _personRepository;
        private readonly Mock<IPersonRepository> _mockPersonRepository;
        private readonly Mock<ICountryRepository> _mockCountryRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly ITestOutputHelper _testOutputHelper;
        //for making mocked objects
        private readonly IFixture _fixture;
        //mocking the logger
        private readonly Mock<ILogger<PersonGetterService>> _mockGetterLogger;
        private readonly Mock<ILogger<PersonAddService>> _mockAddLogger;
        private readonly Mock<ILogger<PersonDeleteService>> _mockDeleteLogger;
        private readonly Mock<ILogger<PersonUpdateService>> _mockUpdateLogger;
        private readonly Mock<ILogger<PersonSortService>> _mockSortLogger;
        //creating logger filed for instancieated instance
        private readonly ILogger<PersonGetterService> _loggerGetterService;
        private readonly ILogger<PersonAddService> _loggerAddService;
        private readonly ILogger<PersonDeleteService> _loggerDeleteService;
        private readonly ILogger<PersonUpdateService> _loggerUpdateService;
        private readonly ILogger<PersonSortService> _loggerSortService;
        //adding richiment to the loggers
        private readonly IDiagnosticContext _diagnosticMessage;
        //mock the it
        private readonly Mock<IDiagnosticContext> _mockDiagnosticMessage;
        public PersonsServicesTest(ITestOutputHelper testOutputHelper)
        {
            //var countriesInit = new List<Country>() { };
            //var personsInit = new List<Person>() { };
            _mockDiagnosticMessage =new();
            _diagnosticMessage=_mockDiagnosticMessage.Object;
            //locating memory for the Mocked objects
            _mockGetterLogger = new Mock<ILogger<PersonGetterService>>();
            _mockAddLogger = new();
            _mockDeleteLogger = new();
            _mockUpdateLogger = new();
            _mockSortLogger = new();
            //mocking the logger object
            _loggerGetterService=_mockGetterLogger.Object;
            _loggerAddService=_mockAddLogger.Object;
            _loggerDeleteService=_mockDeleteLogger.Object;
            _loggerUpdateService = _mockUpdateLogger.Object;
            _loggerSortService = _mockSortLogger.Object;
            //mocking person and Country Repository
            _mockPersonRepository = new Mock<IPersonRepository>();
            _mockCountryRepository=new Mock<ICountryRepository>();
            _countryRepository=_mockCountryRepository.Object;
            _personRepository = _mockPersonRepository.Object;
            
            _countriesGetterService=new CountriesGetterService(_countryRepository);
            _countriesAddService =new CountriesAddService(_countryRepository);
            //instanciating PersonService
            _personGetterService=new PersonGetterService(_personRepository,_loggerGetterService, _diagnosticMessage);
            _personAddService=new PersonAddService(_personRepository, _loggerAddService, _diagnosticMessage);
            _personUpdateService = new PersonUpdateService(_personRepository, _loggerUpdateService, _diagnosticMessage);
            _personDeleteService = new PersonDeleteService(_personRepository, _loggerDeleteService, _diagnosticMessage);
            _personSortService = new PersonSortService(_personRepository, _loggerSortService, _diagnosticMessage);
            _fixture=new Fixture();
            _testOutputHelper=testOutputHelper;
        }
        private async Task<List<PersonAddRequest>> GetPersonsRequestList()
        {
            CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_1 = await _countriesAddService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesAddService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>().With(p => p.CountryID, country_response_1.CountryID).Create();

            PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>().With(p => p.CountryID, country_response_2.CountryID).Create();
            PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>().With(p => p.CountryID, country_response_2.CountryID).Create();
            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };
            return person_requests;
        }

        [Fact]
        //testing if the argument is null while adding the person request
        #region AddPerson
        public async Task AddPerson_NullPersonRequest_ArgumentNullException()
        {
            //arrange
            PersonAddRequest? personAddRequest = null;
            Func<Task> action = async () =>
            {
                await _personAddService.AddPerson(personAddRequest);
            };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }
        //test if the name of the Request is null must return Argument Excecption
        [Fact]
        public async Task AddPerson_NullPersonName_ToBeArgumentException()
        {
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, null as string).Create();
            Person person = personAddRequest.ToPerson();
            _mockPersonRepository.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
            Func<Task> action = async () =>
            {
                await _personAddService.AddPerson(personAddRequest);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }
        [Fact]
        public async Task AddPerson_DateIsLessThan18()
        {
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().With(p => p.DateOfBirth, Convert.ToDateTime("2009-01-01")).Create();
            Person person = personAddRequest.ToPerson();
            _mockPersonRepository.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
            Func<Task> action = async () =>
            {
                await _personAddService.AddPerson(personAddRequest);
            };
            await action.Should().ThrowAsync<ArgumentException>();



        }
        [Fact]
        public async Task AddPerson_PersonFullValidData_successfully()
        {
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().With(p => p.Email, "Example@hotmail.com").Create();
            Person person = personAddRequest.ToPerson();

            _mockPersonRepository.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
            PersonResponse personResponse_AddPerson = await _personAddService.AddPerson(personAddRequest);
            PersonResponse personResponse_excpected = person.ToPersonRespose();
            //making sure ther is no missmatch 
            personResponse_excpected.PersonID=personResponse_AddPerson.PersonID;
            personResponse_AddPerson.Should().Be(personResponse_excpected);
            personResponse_AddPerson.PersonID.Should().NotBe(Guid.Empty);
        }
        #endregion
        #region GetPersonByID
        //if the person id is null
        [Fact]
        public async Task GetPersonById_NullPersonId()
        {
            //Arrange
            Guid? personId = null;
            Person person = new Person();
            _mockPersonRepository.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid?>())).ReturnsAsync(person);
            PersonResponse? personResponse = await _personGetterService.GetPersonById(personId);
            personResponse?.PersonID.Should().Be(null);
        }
        //adding if the the id is valid and exist
        [Fact]
        public async Task GetPersonId_ValidPersonId()
        {

            Person person = _fixture.Build<Person>().With(p => p.Email, "Example@hotmail.com").With(p => p.Country, null as Country)
                .Create();
            PersonResponse personResponse_excpected = person.ToPersonRespose();
            _mockPersonRepository.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid?>())).ReturnsAsync(person);
            //testig the method by comparing the objects
            PersonResponse? personResponse_FromGetPersonID = await _personGetterService.GetPersonById(person.PersonId);
            personResponse_FromGetPersonID.Should().Be(personResponse_excpected);

        }

        #endregion
        #region GetAllperson
        //the Default the list should be empty
        [Fact]
        public async Task GetAllPerson_regular()
        {
            //how to test this get method
            //first creating  list of personRespons using add person
            //and another one to compare fron the actual function wich we are testing 
            //arrrage

            List<Person> person_excpected = new List<Person>()
           {
               _fixture.Build<Person>().With(p=>p.Email,"example@hotmail.com").With(p=>p.Country,null as Country).Create(),
               _fixture.Build<Person>().With(p=>p.Email,"example@hotmail.com").With(p=>p.Country,null as Country).Create(),
               _fixture.Build<Person>().With(p=>p.Email,"example@hotmail.com").With(p=>p.Country,null as Country).Create(),


           };
            //getting the excpeted value to compare
            List<PersonResponse> personResponse_excpected=person_excpected.Select(p => p.ToPersonRespose()).ToList();
            _mockPersonRepository.Setup(temp => temp.GetAllPersons()).ReturnsAsync(person_excpected);
            List<PersonResponse> PerosnList_From_GetAllPersons = await _personGetterService.GetAllPerson();
            //comparing the PersonResponse form add person functio and the one returned from GetallPerson
            //foreach(PersonResponse pesonreponse_from_Addperson in personResponses_List_From_addperson)
            // {
            //     Assert.Contains(pesonreponse_from_Addperson, Personresposne_from_GetAllperson);
            // }
            PerosnList_From_GetAllPersons.Should().BeEquivalentTo(personResponse_excpected);
        }
        #endregion
        #region GetFilteredPersons
        [Fact]
        //if the searchString is empty then all the list must returned
        public async Task GetFilterdPersons_emptySearchString_returnAllPerson_in_the_list()
        {
            List<Person> person_excpected = new List<Person>()
           {
               _fixture.Build<Person>().With(p=>p.Email,"example@hotmail.com").With(p=>p.Country,null as Country).Create(),
               _fixture.Build<Person>().With(p=>p.Email,"example@hotmail.com").With(p=>p.Country,null as Country).Create(),
               _fixture.Build<Person>().With(p=>p.Email,"example@hotmail.com").With(p=>p.Country,null as Country).Create(),


           };
            //getting the excpeted value to compare
            List<PersonResponse> personResponse_excpected = person_excpected.Select(p => p.ToPersonRespose()).ToList();
            _mockPersonRepository.Setup(temp => temp.GetFilterdPerson(It.IsAny<Expression<Func<Person,bool>>>())).ReturnsAsync(person_excpected);
            List<PersonResponse> PerosnList_From_GetFilteredPersons = await _personGetterService.GetFilteredPersons(nameof(Person.PersonName),"");
            //comparing the PersonResponse form add person functio and the one returned from GetallPerson
            //foreach(PersonResponse pesonreponse_from_Addperson in personResponses_List_From_addperson)
            // {
            //     Assert.Contains(pesonreponse_from_Addperson, Personresposne_from_GetAllperson);
            // }
            PerosnList_From_GetFilteredPersons.Should().BeEquivalentTo(personResponse_excpected);
        }
        //searching using person name
        [Fact]
        public async Task GetFilterdPersons_SearchingUsingPersonName_With_nonEmptySearchString()
        {

            List<Person> person_excpected = new List<Person>()
           {
               _fixture.Build<Person>().With(p=>p.Email,"example@hotmail.com").With(p=>p.Country,null as Country).Create(),
               _fixture.Build<Person>().With(p=>p.Email,"example@hotmail.com").With(p=>p.Country,null as Country).Create(),
               _fixture.Build<Person>().With(p=>p.Email,"example@hotmail.com").With(p=>p.Country,null as Country).Create(),


           };
            //getting the excpeted value to compare
            List<PersonResponse> personResponse_excpected = person_excpected.Select(p => p.ToPersonRespose()).ToList();
            _mockPersonRepository.Setup(temp => temp.GetAllPersons()).ReturnsAsync(person_excpected);
            _mockPersonRepository.Setup(temp => temp.GetFilterdPerson(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(person_excpected);
            List<PersonResponse> PerosnList_From_GetFilteredPersons = await _personGetterService.GetFilteredPersons(nameof(Person.PersonName), "sa");
            //comparing the PersonResponse form add person functio and the one returned from GetallPerson
            //foreach(PersonResponse pesonreponse_from_Addperson in personResponses_List_From_addperson)
            // {
            //     Assert.Contains(pesonreponse_from_Addperson, Personresposne_from_GetAllperson);
            // }
            PerosnList_From_GetFilteredPersons.Should().BeEquivalentTo(personResponse_excpected);

        }
        #endregion
        #region GetSortedPerson
        [Fact]
        public async Task SortPerson_With_PersonNameDesc_allPersonInDecendingOrder()
        {
            //getting 

            List<Person> person_excpected = new List<Person>()
           {
               _fixture.Build<Person>().With(p=>p.Email,"example@hotmail.com").With(p=>p.Country,null as Country).Create(),
               _fixture.Build<Person>().With(p=>p.Email,"example@hotmail.com").With(p=>p.Country,null as Country).Create(),
               _fixture.Build<Person>().With(p=>p.Email,"example@hotmail.com").With(p=>p.Country,null as Country).Create(),


           };
           
            _mockPersonRepository.Setup(temp => temp.GetAllPersons()).ReturnsAsync(person_excpected);
            List<PersonResponse>allPeople=await _personGetterService.GetAllPerson();
            List<PersonResponse> PersonFromGetsortedPerson =await _personSortService.GetSortedPersons(allPeople, "PersonName", OrderOptions.DESC);
            PersonFromGetsortedPerson.Should().BeInDescendingOrder(p=>p.PersonName);
        }
        #endregion
        #region UpdaterPerson
        [Fact]
        public async Task UpdatedPersonAddRequest_Null_Update_Request()
        {

            PersonUpdateRequest? personUpdateRequest = null;
            Func<Task> action = async () =>
            {
                await _personUpdateService.UpdatePerson(personUpdateRequest);
            };
            await action.Should().ThrowAsync<ArgumentNullException>();

        }
        [Fact]
        public async Task UpdatedPersonAddRequest_Invalid_PersonId()
        {

            PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest() { PersonId=Guid.NewGuid() };
            Func<Task> action = async () =>
            {
                await _personUpdateService.UpdatePerson(personUpdateRequest);
            };
            await action.Should().ThrowAsync<ArgumentException>();


        }
        [Fact]
        //if the personName is null 
        public async Task UpatedPersonAddRequest_Null_PersonName()
        {
            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse = await _countriesAddService.AddCountry(countryAddRequest);
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().With(p => p.Email, "Example@hotmail.com").With(p => p.CountryID, countryResponse.CountryID).Create();
            PersonResponse personResponse = await _personAddService.AddPerson(personAddRequest);
            PersonUpdateRequest? personUpdateRequest = personResponse.ToPersonUpdateRequest();
            personUpdateRequest.PersonName=null;
            Func<Task> action = async () =>
            {
                await _personUpdateService.UpdatePerson(personUpdateRequest);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }
        [Fact]
        //updating the person name and email
        public async Task UpatedPersonAddRequest_Update_PersonName_email_UpdatedPerson()
        {
            Person person = _fixture.Build<Person>()
       .With(temp => temp.Email, "someone@example.com")
       .With(temp => temp.Country, null as Country)
       .With(temp => temp.Gender, "Male")
       .Create();

            PersonResponse person_response_expected = person.ToPersonRespose();

            PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();

            _mockPersonRepository
             .Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
             .ReturnsAsync(person);

            _mockPersonRepository
             .Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
             .ReturnsAsync(person);
            PersonResponse personResponse_From_UpdatePerson = await _personUpdateService.UpdatePerson(person_update_request);
            personResponse_From_UpdatePerson.Should().BeEquivalentTo(person_response_expected);
        }
        #endregion
        #region DeletePreson
        //if the Person Id is Invalid it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonId()
        {

            bool DeletedPeson = await _personDeleteService.DeletePerson(Guid.NewGuid());
            DeletedPeson.Should().BeFalse();
        }
        //if the person delete it should return True
        [Fact]
        public async Task DeletePerson_DeletePersonSuccefully_retturnTrue()
        {
            Person person = _fixture.Build<Person>().With(p=>p.Country, null as Country).Create();
            _mockPersonRepository.Setup(p => p.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(person);
            bool isDeleted =await _personDeleteService.DeletePerson(person.PersonId);
            isDeleted.Should().BeTrue();
        }

        #endregion
    }
}
