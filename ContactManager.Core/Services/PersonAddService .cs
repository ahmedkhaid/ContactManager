using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using IRepositoryContract;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Serilog;
using SerilogTimings;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Formats.Asn1;
using System.Globalization;
using System.IO.Pipes;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using CustomException;
namespace Services
{
    public class PersonAddService : IPersonsAddService
    {
        //that mine please focus
        private readonly IPersonRepository _PersonRepository;
        private readonly ILogger<PersonAddService> _looger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonResponse ConverFromPersonToPersonResponse(Person person)
        {
            PersonResponse response = person.ToPersonRespose();
            response.Country=person.Country?.CountryName;
            return response;
        }
        public PersonAddService(IPersonRepository personDbContext, ILogger<PersonAddService> logger,IDiagnosticContext diagnosticContext)
        {
            _PersonRepository=personDbContext;
            _looger=logger;
            _diagnosticContext=diagnosticContext;
        }
        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if(personAddRequest==null)
            { throw new ArgumentNullException(); }
            //validate the attribute of the PersonAddRequest PersonName is null
            if(personAddRequest.PersonName==null)
            {
                throw new ArgumentException();
            }
            //validation of the Person request
            ValidationHelper.ModelValidation(personAddRequest);
             Person person = personAddRequest.ToPerson();
            //creating Preson id
            person.PersonId=Guid.NewGuid();

           await _PersonRepository.AddPerson(person);
            //convert from Person to PersonResponse and addding the CountryName using GetCountry id method
            return person.ToPersonRespose();
        }

        
    }
}
