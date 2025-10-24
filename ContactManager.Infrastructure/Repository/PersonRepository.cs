using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRepositoryContract;
using Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Repository
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ILogger<PersonRepository> _logger;
        private readonly PersonDbContext _db;
        public PersonRepository(PersonDbContext personDbContext,ILogger<PersonRepository>logger)
        {
            _logger = logger;
            _db=personDbContext;
        }
        public async Task<Person> AddPerson(Person person)
        {
            await _db.Persons.AddAsync(person);
           await _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePerson(Person person)
        {
            int count = _db.Persons.Count(p => p.PersonId==person.PersonId);
            _db.Persons.RemoveRange(person);
            await _db.SaveChangesAsync();
            return  count > 0;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            _logger.LogInformation("the getallPerson from the Person repository");
         return await _db.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilterdPerson(Expression<Func<Person, bool>> perdicate)
        {
           return await _db.Persons.Where(perdicate).ToListAsync();
        }

        public async Task<Person> GetPersonByPersonId(Guid? personId)
        {

            return await _db.Persons.Include("Country").FirstOrDefaultAsync(p => p.PersonId==personId);
        }

        public async Task<List<Person>> GetSortedPerson(Expression<Func<Person, bool>> perdicate)
        {
            return await _db.Persons.Where(perdicate).ToListAsync();
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person ?personToUpdate = _db.Persons.Where(p => p.PersonId==person.PersonId).FirstOrDefault();
            if (personToUpdate==null)
            {
                return person;
            }
            personToUpdate.PersonName=person.PersonName;
            personToUpdate.Gender=person.Gender;
            personToUpdate.Address=person.Address;
            personToUpdate.ReceiveNewsLetters=person.ReceiveNewsLetters;
            personToUpdate.Country=person.Country;
            personToUpdate.DateOfBirth=person.DateOfBirth;
            personToUpdate.Email=person.Email;
            personToUpdate.CountryID=person.CountryID;
            await _db.SaveChangesAsync();
            return personToUpdate;
        }
    }
}
