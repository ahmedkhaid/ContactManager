using Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;
using FluentAssertions;
using IRepositoryContract;
using Microsoft.Extensions.Options;
using System.Linq;
namespace TestProject2
{
    public class CountriesServiceTest
    {
        private readonly ICountriesGetterService _countriesGetterService;
        private readonly ICountriesAddService _countriesAddService;
        private readonly ICountriesUploadService _countriesUploadService;
        private readonly IFixture _fixture;
        private readonly ICountryRepository _countryRepository;
        private readonly Mock<ICountryRepository> _countryResponseMock;

        public CountriesServiceTest()
        {
            //mocking the ICountry repository 
            _countryResponseMock = new Mock<ICountryRepository>();

            _countryRepository=_countryResponseMock.Object;
            //acting as in memory database
            _fixture = new Fixture();
            // Pass mocked dbContext into your service
            _countriesGetterService = new CountriesGetterService(_countryRepository);
            _countriesAddService = new CountriesAddService(_countryRepository);
            _countriesUploadService = new CountriesUploadService(_countryRepository);


        }
        #region AddCountry

        //When CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            // {
            //     //Act
            //     await _countriesService.AddCountry(request);
            // });
            //using fluent assertion 
            Func<Task> action = async () =>
            {
                await _countriesAddService.AddCountry(request);
            };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //When the CountryName is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>().With(c => c.CountryName, null as string).Create();

            //Assert
            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            // {
            //     //Act
            //     await _countriesService.AddCountry(request);
            // });
            Func<Task> action = async () =>
            {
                await _countriesAddService.AddCountry(request);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }


        //When the CountryName is duplicate, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            //Arrange
            //adding two country object with the same coutryname
            CountryAddRequest country_request_1 = _fixture.Build<CountryAddRequest>().With(c => c.CountryName,"test").
                Create();
            CountryAddRequest country_request_2 = _fixture.Build<CountryAddRequest>().With(c => c.CountryName,"test").
               Create();
            Country country_1 = country_request_1.ToCountry();
            Country country_2= country_request_2.ToCountry();
            _countryResponseMock.Setup(c=>c.AddCountry(It.IsAny<Country>())).ReturnsAsync(country_1);
            _countryResponseMock.Setup(c=>c.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(null as Country);
            Func<Task> action = async () =>
            {
                _countryResponseMock.Setup(c => c.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(country_1);
                await _countriesAddService.AddCountry(country_request_2);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }


        //When you supply proper country name, it should insert (add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();
            Country country = country_request.ToCountry();
            CountryResponse country_response = country.ToCountryResponse();

            _countryResponseMock
             .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
             .ReturnsAsync(country);

            _countryResponseMock
             .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
             .ReturnsAsync(null as Country);


            //Act
            CountryResponse country_from_add_country = await _countriesAddService.AddCountry(country_request);

            country.CountryID = country_from_add_country.CountryID;
            country_response.CountryID = country_from_add_country.CountryID;

            //Assert
            country_from_add_country.CountryID.Should().NotBe(Guid.Empty);
            country_from_add_country.Should().BeEquivalentTo(country_response);
        }
        #endregion
        #region GetAllCountries
        //the default the list should be empty before adding any country
        [Fact]
        public async Task GetAllCountry_EmptyList()
        {
            List<Country> emptyList = new List<Country>();
            _countryResponseMock.Setup(c => c.GetAllCountries()).ReturnsAsync(emptyList);
            List<CountryResponse> CountryResponse_From_GetALlCountries = await _countriesGetterService.GetAllCountries();
            CountryResponse_From_GetALlCountries.Should().BeEmpty();
        }
        [Fact]
        public async Task GetAllCountry_CountiesContainsActualCountriesAdded()
        {
            List<Country> countriesList = new List<Country>()
           {
                _fixture.Build<Country>().With(c=>c.persons,new List<Person>()).Create(),
                _fixture.Build<Country>().With(c=>c.persons,new List<Person>()).Create(),
                _fixture.Build<Country>().With(c=>c.persons,new List<Person>()).Create()

            };
            List<CountryResponse> CountryResponse_expected = countriesList.Select(c => c.ToCountryResponse()).ToList();
            _countryResponseMock.Setup(c => c.GetAllCountries()).ReturnsAsync(countriesList);
            List<CountryResponse> Countries_from_GetAllCountries = await _countriesGetterService.GetAllCountries();
            Countries_from_GetAllCountries.Should().BeEquivalentTo(CountryResponse_expected);
        }
        #endregion
        #region GetCountryById
        //if the Country id is null
        [Fact]
        public async Task GetCountryById_NullId()
        {
            Guid? guid = null;
            CountryResponse? response = await _countriesGetterService.GetCountryById(guid);
            response?.Should().BeNull();
        }
        [Fact]
        public async Task GetCountryById_ValidCountryId()
        {
            //adding the country to the list
            //then testing the function if the get all country contains the resposne from the function it should pass
            CountryAddRequest request = _fixture.Create<CountryAddRequest>();
            Country country = request.ToCountry();
            CountryResponse countryResponse_Exepected = country.ToCountryResponse();
            _countryResponseMock.Setup(c => c.GetCountryById(It.IsAny<Guid>())).ReturnsAsync(country);
            CountryResponse? countryResponse_Actual = await _countriesGetterService.GetCountryById(country.CountryID);
            countryResponse_Actual.Should().BeEquivalentTo(countryResponse_Exepected);
        }
        //getting the County response if teh ==
        #endregion

    }
}
