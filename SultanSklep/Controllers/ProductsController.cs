using Microsoft.AspNetCore.Mvc;
using SultanSklep.DataAccessLayer;
using SultanSklep.Models;
using System.Linq;

namespace SultanSklep.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // Məhsullar siyahısı
        public IActionResult Index()
        {
            var products = _context.Products
                .Where(p => !p.IsDeleted && p.IsAvailable)
                .ToList();
            return View(products);
        }

        // Məhsul detalları
        public IActionResult ProductDetails(int id)
        {
            var product = _context.Products
                .FirstOrDefault(p => p.Id == id && !p.IsDeleted && p.IsAvailable);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}