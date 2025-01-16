using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SultanSklep.ViewModels.Product
{
    public class ProductListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public string CategoryName { get; set; }
        public string Photo { get; set; }
    }
}
