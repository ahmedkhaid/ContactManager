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
    public class PersonGetterService : IPersonsGetterService
    {
        //that mine please focus
        private readonly IPersonRepository _PersonRepository;
        private readonly ILogger<PersonGetterService> _looger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonResponse ConverFromPersonToPersonResponse(Person person)
        {
            PersonResponse response = person.ToPersonRespose();
            response.Country=person.Country?.CountryName;
            return response;
        }
        public PersonGetterService(IPersonRepository personDbContext, ILogger<PersonGetterService>logger,IDiagnosticContext diagnosticContext)
        {
            _PersonRepository=personDbContext;
            _looger=logger;
            _diagnosticContext=diagnosticContext;
        }

        public async Task <List<PersonResponse>> GetAllPerson()
        {
            //List<PersonResponse>personResponses = _persons.Select(p=>p.ToPersonRespose()).ToList();
            var personList = await _PersonRepository.GetAllPersons();
            //return personResponses;
            return personList.Select(personslist => personslist.ToPersonRespose()).ToList();
        }

        public async Task<PersonResponse?> GetPersonById(Guid? PersonId)
        {
            if(PersonId==null)
            {
                return null;
            }
            Person? person = await _PersonRepository.GetPersonByPersonId(PersonId);
            if(person==null)
            {
                throw new ArgumentException();
            }

            return person.ToPersonRespose();
        }

        public  async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            _looger.LogInformation("the filterd person method in the personService");

            var personList = await _PersonRepository.GetAllPersons();

            if (searchBy==null||searchString==null)
            {
                
                //return personResponses;
                return personList.Select(personslist => personslist.ToPersonRespose()).ToList();
            }
            List<Person> personAfterFeltering = null;
            using (Operation.Time("Time for calling data base"))
            { 
                personAfterFeltering = searchBy switch
                {
                    (nameof(PersonResponse.PersonName)) => await _PersonRepository.GetFilterdPerson(temp => temp.PersonName.Contains(searchString)),
                    (nameof(PersonResponse.Email)) => await _PersonRepository.GetFilterdPerson(temp => temp.Email.Contains(searchString)),
                    (nameof(PersonResponse.Address)) => await _PersonRepository.GetFilterdPerson(temp => temp.Address.Contains(searchString)),

                    (nameof(PersonResponse.Gender)) => await _PersonRepository.GetFilterdPerson(temp => temp.Gender.Equals(searchString)),
                    (nameof(PersonResponse.Country)) => await _PersonRepository.GetFilterdPerson(temp => temp.Country.CountryName.Contains(searchString)),
                    (nameof(PersonResponse.DateOfBirth)) => await _PersonRepository.GetFilterdPerson(temp => temp.DateOfBirth.Value.ToString().Contains(searchString)),
                    _ => personList

                };
            }
            _diagnosticContext.Set("Persons", personList);
           return personAfterFeltering.Select(p=>p.ToPersonRespose()).ToList();
       

        }
        /// <summary>
        /// sort the list of person have three argument list want to sort the properity to sort by and type of sorting desc or asec
        /// </summary>
        /// <param name="People">list to sort</param>
        /// <param name="sorteBy">the name of proprity to sort by</param>
        /// <param name="orderOption">enum asec or desc</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>


        //getting the Presons as Csv file
        public async Task<MemoryStream> GetCsv()
        {
            MemoryStream csvMemoryStream = new MemoryStream();
            StreamWriter streamWritter=new StreamWriter(csvMemoryStream);
            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            CsvWriter csvWriter = new CsvWriter(streamWritter,csvConfiguration);
            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Gender));
            csvWriter.WriteField(nameof(PersonResponse.Country));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
            csvWriter.NextRecord();
            List<PersonResponse> people =  (await _PersonRepository.GetAllPersons()).Select(p=>p.ToPersonRespose()).ToList();
            foreach (PersonResponse person in people)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Email);
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.Gender);
                csvWriter.WriteField(person.Country);
                csvWriter.WriteField(person.Address);
                if (person.DateOfBirth.HasValue)
                    csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyy-mm-dd"));
                else
                    csvWriter.WriteField("");
                csvWriter.WriteField(person.ReceiveNewsLetters);
                csvWriter.NextRecord();
                csvWriter.Flush();
            }
            csvMemoryStream.Position = 0;
            return csvMemoryStream;
        }
        public async Task<MemoryStream> GetPeopleExcel()
        {
           MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage =new ExcelPackage(memoryStream))
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("personSheet");
                excelWorksheet.Cells["A1"].Value=nameof(PersonResponse.PersonName);
                excelWorksheet.Cells["B1"].Value=nameof(PersonResponse.Email);
                excelWorksheet.Cells["C1"].Value=nameof(PersonResponse.DateOfBirth);
                excelWorksheet.Cells["D1"].Value=nameof(PersonResponse.Age);
                excelWorksheet.Cells["E1"].Value=nameof(PersonResponse.Address);
                excelWorksheet.Cells["F1"].Value=nameof(PersonResponse.Gender);
                excelWorksheet.Cells["G1"].Value=nameof(PersonResponse.Country);
                excelWorksheet.Cells["H1"].Value=nameof(PersonResponse.ReceiveNewsLetters);
                using (ExcelRange headerCells = excelWorksheet.Cells["A1:H1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headerCells.Style.Font.Bold = true;
                }
                int row = 2;
                List<PersonResponse> people = (await _PersonRepository.GetAllPersons()).Select(p => p.ToPersonRespose()).ToList();
                foreach (PersonResponse person in people)
                {
                    excelWorksheet.Cells[row, 1].Value=person.PersonName;
                    excelWorksheet.Cells[row, 2].Value=person.Email;
                    if (person.DateOfBirth.HasValue)
                        excelWorksheet.Cells[row, 3].Value=person.DateOfBirth;
                    else
                        excelWorksheet.Cells[row, 3].Value="";
                        excelWorksheet.Cells[row, 4].Value=person.Age;
                    excelWorksheet.Cells[row, 5].Value=person.Address;
                    excelWorksheet.Cells[row, 6].Value=person.Gender;
                    excelWorksheet.Cells[row, 7].Value=person.Country;
                    excelWorksheet.Cells[row, 8].Value=person.ReceiveNewsLetters;
                    row++;
                 
                }
                excelWorksheet.Cells[$"A1:H{row}"].AutoFitColumns();
                await excelPackage.SaveAsync();
                memoryStream.Position = 0;
                return memoryStream;
            }
        }
    }
}
