using Entities;
using System.Linq.Expressions;

namespace IRepositoryContract
{
    public interface IPersonRepository
    {
        /// <summary>
        /// adding person to the database
        /// </summary>
        /// <param name="person">person to add</param>
        /// <returns>person after adding</returns>
        Task<Person> AddPerson(Person person);
        Task<Person>GetPersonByPersonId(Guid ?personId);
        /// <summary>
        /// Delete given person
        /// </summary>
        /// <param name="person">person to delete</param>
        /// <returns>true if delete successfully </returns>
        Task<Boolean>DeletePerson(Person person);
        /// <summary>
        /// return the list of person 
        /// </summary>
        /// <returns></returns>
        Task<List<Person>> GetAllPersons();
        /// <summary>
        /// update the given person
        /// </summary>
        /// <param name="person">person to update</param>
        /// <returns>person after updated</returns>
        Task<Person> UpdatePerson(Person person);
        Task<List<Person>> GetSortedPerson(Expression<Func<Person, bool>> predicate);
        Task<List<Person>> GetFilterdPerson(Expression <Func<Person,bool>> perdicate);
       
    }
}
