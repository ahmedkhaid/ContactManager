using ServiceContracts.DTO;
using System;
using ServiceContracts.Enums;
namespace ServiceContracts
{
    public interface IPersonsGetterService
    {
        public Task<List<PersonResponse>> GetAllPerson();
        public Task<PersonResponse?> GetPersonById(Guid? PersonId);
        public Task<List<PersonResponse>> GetFilteredPersons(string searchBy,string?searchString);
        public Task<MemoryStream> GetCsv();
        ///<summary>retrun people as Excel file</summary>>
        public Task<MemoryStream> GetPeopleExcel();
    }
}
