using Entities;

namespace IRepositoryContract
{
    public interface ICountryRepository
    {
        /// <summary>
        /// 
        /// adding Give country to the list
        /// </summary>
        /// <param name="country">Country to add</param>
        /// <returns>Country that was added</returns>
         Task<Country> AddCountry(Country country);
        /// <summary>
        /// return all countries
        /// 
        /// </summary>
        /// <returns></returns>
         Task<List<Country>> GetAllCountries();
        /// <summary>
        /// return country object based on the given id
        /// </summary>
        /// <param name="id">the country id to search</param>
        /// <returns></returns>
         Task<Country>GetCountryById(Guid? id);
        /// <summary>
        /// Returns a country object based on the given country name
        /// </summary>
        /// <param name="countryName">Country name to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByCountryName(string countryName);
    }
}
