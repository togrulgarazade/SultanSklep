using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SultanSklep.Models
{

    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

    }
}
