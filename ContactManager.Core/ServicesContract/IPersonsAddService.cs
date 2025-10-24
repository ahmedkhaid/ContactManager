using ServiceContracts.DTO;
using System;
using ServiceContracts.Enums;
namespace ServiceContracts
{
    public interface IPersonsAddService{
      
        public Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

     
    }
}
