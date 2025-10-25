using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ContactManager.Core.DTO
{
    public  class RegisterDTO
    {
        [Required(ErrorMessage ="the PersonName can't be null")]
       public  string ? PersonName {  get; set; }
        [Required(ErrorMessage = "the Phone can't be Empty")]
        [RegularExpression("^[0-9]*$",ErrorMessage ="The password Can't be Empty")]
        [DataType(DataType.PhoneNumber)]
         public string ?Phone {  get; set; }
        [Required(ErrorMessage = "the Email can't be null")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "the Password can't be null")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "the confirmedPassword can't be null")]
        [Compare("Password",ErrorMessage ="the confirmed password Should be equal to Password")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

    }
}
