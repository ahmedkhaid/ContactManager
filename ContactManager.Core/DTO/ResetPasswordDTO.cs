using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Core.DTO
{
    public class ResetPasswordDTO
    {
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }
        [Required]
        public string ?Code { get; set; }
    }
}
