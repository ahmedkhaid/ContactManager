using IRepositoryContract;
using Entities;
using Microsoft.EntityFrameworkCore;
namespace Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly PersonDbContext _db;
        public CountryRepository(PersonDbContext dbContext)
        {
            _db = dbContext;
        }
        public async Task<Country> AddCountry(Country country)
        {
             _db.Countries.Add(country);
            await _db.SaveChangesAsync();
            return country;
        }

        public async Task<List<Country>> GetAllCountries()
        {
            List<Country> countries = await _db.Countries.ToListAsync();
            return countries;
        }

        public async Task<Country?> GetCountryByCountryName(string countryName)
        {
            Country? country = await _db.Countries.FirstOrDefaultAsync(c=>c.CountryName==countryName);
            return country;
        }

        public async Task<Country> GetCountryById(Guid? id)
        {
            return await _db.Countries.FirstAsync(c => c.CountryID==id);

           
        }

       
    }
}
