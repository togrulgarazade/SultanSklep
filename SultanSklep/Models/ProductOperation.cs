using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SultanSklep.Models
{
    public class ProductOperation
    {
        public int Id { get; set; }
        public string UserID { get; set; }
        public int? AddressID { get; set; } // Nullable edilərək AddToCart zamanı tələb edilməyəcək
        public int ProductID { get; set; }
        public string OrderNumber { get; set; }
        public bool InCart { get; set; }
        public bool IsOrdered { get; set; }
        public bool IsPending { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsDeleted { get; set; }
        public int Count { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Address Address { get; set; }
        public virtual Product Product { get; set; }
    }

}
