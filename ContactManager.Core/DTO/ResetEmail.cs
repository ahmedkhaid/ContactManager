using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Core.DTO
{
    public class ResetEmail
    {
        [Required(ErrorMessage ="The email cant be null")]
       public string? Email {  get; set; }
    }
}
