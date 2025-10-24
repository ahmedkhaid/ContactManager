using Entities;
using ServiceContracts.Enums;
using System;


namespace ServiceContracts.DTO
{
    public class PersonResponse
    {  
        public Guid PersonID { get; set; }
      
        public string? PersonName { get; set; }
    
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public double? Age { get; set; }
        public override bool Equals(object? obj)
        {
           if (obj == null) return false;
           if(obj.GetType()!=typeof(PersonResponse))
                return false;
           PersonResponse personResponse= (PersonResponse)obj;
            return PersonID == personResponse.PersonID&&PersonName==personResponse.PersonName&&Email==personResponse.Email&&
                Address==personResponse.Address&&CountryID==personResponse.CountryID&&ReceiveNewsLetters==personResponse.ReceiveNewsLetters;
        }
        public override string ToString()
        {
            return $"PersonID: {PersonID} PersonName :{PersonName} Email : {Email} DateOfBirth {DateOfBirth}" +
                $"Gender :{Gender} CountryID : {CountryID} Country{Country} Address :{Address} ReceiveNewsLetters :{ReceiveNewsLetters} Age :{Age}";
        }
        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest() { PersonId=this.PersonID, Address=Address, CountryID=CountryID, DateOfBirth=DateOfBirth, Gender=(GenderOptions)Enum.Parse(typeof(GenderOptions),Gender, true), Email=Email, PersonName=PersonName, ReceiveNewsLetters=ReceiveNewsLetters };
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
    public static class PersonExtention
    {
        public static PersonResponse ToPersonRespose(this Person person)
        {
            return new PersonResponse()
            {
                PersonID=person.PersonId,
                PersonName=person.PersonName,
                Email=person.Email,
                DateOfBirth=person.DateOfBirth,
                Gender=person.Gender,
                CountryID=person.CountryID,
                Address=person.Address,
                ReceiveNewsLetters=person.ReceiveNewsLetters,
                Age=(person.DateOfBirth!=null) ? Math.Round((DateTime.Today-person.DateOfBirth.Value).TotalDays/365.25) : null,
                Country=person.Country?.CountryName
            };
    }
    }
}
