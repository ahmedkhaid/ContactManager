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
    public class PersonUpdateService : IPersonUpdateService
    {
        //that mine please focus
        private readonly IPersonRepository _PersonRepository;
        private readonly ILogger<PersonUpdateService> _looger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonResponse ConverFromPersonToPersonResponse(Person person)
        {
            PersonResponse response = person.ToPersonRespose();
            response.Country=person.Country?.CountryName;
            return response;
        }
        public PersonUpdateService(IPersonRepository personDbContext, ILogger<PersonUpdateService> logger,IDiagnosticContext diagnosticContext)
        {
            _PersonRepository=personDbContext;
            _looger=logger;
            _diagnosticContext=diagnosticContext;
        }
    
        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            
           //check if the personUpdate is not null
           if (personUpdateRequest==null)
            {
                throw new ArgumentNullException(nameof(personUpdateRequest));
            }
           //validate the object
            ValidationHelper.ModelValidation(personUpdateRequest);
           
            if(personUpdateRequest.PersonName==null)
            {
                throw new ArgumentException(nameof(personUpdateRequest));
            }
            Person ?matchingPerson = await _PersonRepository.GetPersonByPersonId(personUpdateRequest.PersonId);
            if(matchingPerson==null)
            {
                throw new InvalidPersonIdException("the personId cant be null");
            }
            //update all details
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
           await _PersonRepository.UpdatePerson(matchingPerson);
          return  matchingPerson.ToPersonRespose();
        }

    }
}
