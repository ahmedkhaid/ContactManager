using ServiceContracts.DTO;
using System;
using ServiceContracts.Enums;
using Microsoft.Extensions.Logging;
namespace ServiceContracts
{
    public interface IPersonsDeleteService
    {
     
        public Task<bool> DeletePerson(Guid? PersonId);
    
    }
}
