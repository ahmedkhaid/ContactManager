using Entities;
using System;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;
namespace ServiceContracts.DTO
{
    public class PersonAddRequest
    {
        [Required(ErrorMessage ="the Name cant be empty")]
        public string? PersonName { get; set; }
        [EmailAddress(ErrorMessage ="the email must be in proper format")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage ="the email cant be empty")]
        public string? Email { get; set; }
        [DataType(DataType.Date)]
     
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "Please select One gender")]
        public GenderOptions? Gender { get; set; }
        [Required]
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
 
        public Person ToPerson()
        {
           
            return new Person() { PersonId=Guid.NewGuid(), PersonName=PersonName, Email=Email, DateOfBirth=DateOfBirth, Gender=Gender.ToString(), CountryID=CountryID, Address=Address, ReceiveNewsLetters=ReceiveNewsLetters };
        }
    }
}
