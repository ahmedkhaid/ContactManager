using ServiceContracts.DTO;
using System;
using ServiceContracts.Enums;
namespace ServiceContracts
{
    public interface IPersonUpdateService
    {
       
        public Task<PersonResponse> UpdatePerson(PersonUpdateRequest? updated);
      
    }
}
