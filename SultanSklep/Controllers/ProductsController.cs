using Microsoft.AspNetCore.Mvc;
using SultanSklep.DataAccessLayer;
using SultanSklep.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SultanSklep.ViewModels.Product;

namespace SultanSklep.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            var productViewModel = new ProductViewModel()
            {
                Product = await _context.Products.ToListAsync(),
            };

            return View(productViewModel);

        }
        public async Task<IActionResult> ProductDetails(int id)
        {
            if (id == 0)
            {
                return BadRequest("Invalid product ID.");
            }

            var product = await _context.Products
                .Where(p => p.Id == id)
                .Include(p => p.Category) // Əgər kateqoriyanı da göstərmək istəyirsənsə
                .SingleOrDefaultAsync();

            if (product == null)
            {
                return NotFound("No product found with the given ID.");
            }

            var productViewModel = new ProductViewModel()
            {
                Id = product.Id,   
                ProductName = product.ProductName,
                Price = product.Price,
                Description = product.Description,
                CategoryName = product.Category != null ? product.Category.Name : "No Category",
                Count = product.Count,
                Image = product.Image,



                Product = await _context.Products.ToListAsync()
            };

            return View(productViewModel);
        }

    }
}