using Entities;
using Microsoft.AspNetCore.Http;

using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IRepositoryContract;
namespace Services
{
    public class CountriesAddService : ICountriesAddService
    {
        private readonly ICountryRepository _countriesRepository;
        public CountriesAddService(ICountryRepository db)
        {
            _countriesRepository = db;
        
        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            //Validation: countryAddRequest parameter can't be null
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            //Validation: CountryName can't be null
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            //Validation: CountryName can't be duplicate
            if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
            {
                throw new ArgumentException("Given country name already exists");
            }

            //Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();

            //generate CountryID
            country.CountryID = Guid.NewGuid();

            //Add country object into _countries
            await _countriesRepository.AddCountry(country);

            return country.ToCountryResponse();
        }
    }
}

