using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SultanSklep.Models
{
    public class Address
    {
        public int Id { get; set; }

        public string AddressLabel { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string MNumber { get; set; }

        public string Flat { get; set; }

        public string PostNumber { get; set; }

        public string UserID { get; set; }

        public virtual ApplicationUser User { get; set; } // ApplicationUser modeli istifadə olunur
    }
}
