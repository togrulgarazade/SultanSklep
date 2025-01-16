using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SultanSklep.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Ad 100 simvoldan artıq ola bilməz.")]
        public string ProductName { get; set; }

        [StringLength(500, ErrorMessage = "Təsvir 500 simvoldan artıq ola bilməz.")]
        public string Description { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Sayı sıfırdan az ola bilməz.")]
        public int Count { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Qiymət sıfırdan az ola bilməz.")]
        public decimal Price { get; set; } // Yeni əlavə edilən xassə

        public string Image { get; set; } // Şəkilin URL-i və ya yolu

        [NotMapped]
        public IFormFile Photo { get; set; } // Şəkil yükləmə üçün

        [Required]
        public int CategoryId { get; set; } // Xarici açar

        public Category Category { get; set; } // Kateqoriya obyektinə naviqasiya

        public bool IsDeleted { get; set; } // Məhsulun silinmiş olduğunu göstərir

        public bool IsAvailable { get; set; } // Məhsulun mövcud olub olmadığını göstərir

        public List<ProductImage> Images { get; set; } = new List<ProductImage>(); // Çoxlu şəkil saxlamaq üçün
    }

}
