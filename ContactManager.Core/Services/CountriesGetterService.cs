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
    public class CountriesGetterService : ICountriesGetterService
    {
        private readonly ICountryRepository _countriesRepository;
        public CountriesGetterService(ICountryRepository db)
        {
            _countriesRepository = db;
        
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            List<Country>countries= await _countriesRepository.GetAllCountries();
                return  countries.Select(c=>c.ToCountryResponse()).ToList();
        }
      
        public async Task<CountryResponse?> GetCountryById(Guid? id)
        {
            if(id==null)
            {
                return null;
            }
            Country? country = await _countriesRepository.GetCountryById(id);
            if (country==null)
                return null;
            return country.ToCountryResponse();
        }
    }
}

