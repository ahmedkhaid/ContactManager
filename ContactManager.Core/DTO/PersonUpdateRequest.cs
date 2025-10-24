using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class PersonUpdateRequest
    {
        [Required]
        public Guid PersonId { get; set; }
        [Required(ErrorMessage = "the name cant be empty")]
        public string? PersonName { get; set; }
        [EmailAddress(ErrorMessage = "the email must be in proper format")]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public Person ToPerson()
        {
           
            return new Person() { PersonId=PersonId, PersonName=PersonName, Email=Email, DateOfBirth=DateOfBirth, Gender=Gender.ToString(), CountryID=CountryID, Address=Address, ReceiveNewsLetters=ReceiveNewsLetters };
        }

    }
}
