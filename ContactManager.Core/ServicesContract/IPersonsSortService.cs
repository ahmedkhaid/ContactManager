using ServiceContracts.DTO;
using System;
using ServiceContracts.Enums;
namespace ServiceContracts
{
    public interface IPersonsSortService
    {
      
        public Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse>People, string sorteBy, OrderOptions orderOption);
     
    }
}
