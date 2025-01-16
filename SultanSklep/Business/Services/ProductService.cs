using Microsoft.EntityFrameworkCore;
using SultanSklep.DataAccessLayer;
using SultanSklep.Models;
using SultanSklep.ViewModels.Product;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SultanSklep.Business.Utilites;

namespace Business.Services
{
    public class ProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateProductAsync(ProductCreateViewModel productViewModel)
        {
            // Create the Product entity
            var product = new Product
            {
                ProductName = productViewModel.Name,
                Description = productViewModel.Description,
                Price = productViewModel.Price,
                Count = productViewModel.Count,
                CategoryId = productViewModel.CategoryId,
                IsDeleted = false,
                IsAvailable = true,
                Images = new List<ProductImage>()
            };

            // Add product to database
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Save and associate images
            if (productViewModel.Photos != null && productViewModel.Photos.Any())
            {
                foreach (var photo in productViewModel.Photos)
                {
                    string fileName = await photo.SaveFileAsync("wwwroot", "assets", "img");

                    product.Images.Add(new ProductImage
                    {
                        Image = fileName,
                        ProductId = product.Id,
                        IsMain = product.Images.Count == 0 // İlk şəkil əsasdır
                    });
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
