using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SultanSklep.ViewModels.Product
{
    public class ProductCreateViewModel
    {
        public string Name { get; set; } // Məhsulun adı
        public string Description { get; set; } // Məhsulun təsviri
        public decimal Price { get; set; } // Məhsulun qiyməti
        public int Count { get; set; } // Məhsul sayı
        public int CategoryId { get; set; } // Kateqoriya ID
        public List<IFormFile> Photos { get; set; } // Çoxlu şəkil yükləmə
    }
}
