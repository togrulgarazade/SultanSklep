using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SultanSklep.ViewModels.AccountViewModels
{
    public class RegisterViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
