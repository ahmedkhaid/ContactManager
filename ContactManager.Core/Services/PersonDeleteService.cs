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
    public class PersonDeleteService : IPersonsDeleteService
    {
        //that mine please focus
        private readonly IPersonRepository _PersonRepository;
        private readonly ILogger<PersonDeleteService> _looger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonResponse ConverFromPersonToPersonResponse(Person person)
        {
            PersonResponse response = person.ToPersonRespose();
            response.Country=person.Country?.CountryName;
            return response;
        }
        public PersonDeleteService(IPersonRepository personDbContext, ILogger<PersonDeleteService>logger,IDiagnosticContext diagnosticContext)
        {
            _PersonRepository=personDbContext;
            _looger=logger;
            _diagnosticContext=diagnosticContext;
        }


        public async Task<bool> DeletePerson(Guid? PersonId)
        {
           if (PersonId==null)
            {
                throw new ArgumentNullException();
            }
            Person? person =await _PersonRepository.GetPersonByPersonId(PersonId);
            if(person==null)
            {
                return false;
            }
          await  _PersonRepository.DeletePerson(person);
            return true;
        }
        //getting the Presons as Csv file
      
    }
}
