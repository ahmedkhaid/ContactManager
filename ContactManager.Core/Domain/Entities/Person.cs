using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities
{
    public class Person
    {
        [Key]
        public Guid PersonId {  get; set; }
        [StringLength(20)]
        public string? PersonName {  get; set; }
        [EmailAddress]
        public string? Email { get; set; }

        public DateTime? DateOfBirth { get; set; }
        [StringLength(10)]
        public string? Gender { get; set; }
        //uniqueCode
        public Guid? CountryID { get; set; }
        [StringLength(120)]
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public string? TIN { get; set; }
        [ForeignKey("CountryID")]
        public virtual Country? Country { get; set; }//can add the specification using FluentApi
        public override string ToString()
        {
            return $"Person Name {PersonName}, Country {Country}, Address {Address}," +
                $"Date Of Birth {DateOfBirth?.ToString("yy-mm-ddd")}";
        }
    }
}
