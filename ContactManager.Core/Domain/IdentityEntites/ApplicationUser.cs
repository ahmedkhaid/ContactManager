using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Core.Domain.IdentityEntites
{
    public class ApplicationUser:IdentityUser<Guid>
    {
        string? PersonName {  get; set; }
    }
}
