using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SultanSklep.DataAccessLayer;
using SultanSklep.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SultanSklep.ViewModels.Product;

namespace SultanSklep.Controllers
{
    public class ProductOperationsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductOperationsController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Cart()
        {
            var userId = User.Identity.Name; // Daxil olan istifadəçinin adı və ya ID

            // İstifadəçinin səbətindəki məhsulları çəkirik
            var productOperations = await _context.ProductOperations
                .Where(po => po.UserID == userId && po.InCart == true)
                .Include(po => po.Product) // Məhsul məlumatlarını da əlavə et
                .ToListAsync();

            // Əgər səbət boşdursa
            if (!productOperations.Any())
            {
                return View(new ProductViewModel
                {
                    ProductsInCart = new List<ProductOperation>() // Boş səbət
                });
            }

            // Məhsul məlumatlarını ViewModel-ə yükləyirik
            ProductViewModel productViewModel = new ProductViewModel
            {
                ProductsInCart = productOperations
            };

            return View(productViewModel);
        }


        [Authorize]
        public async Task<IActionResult> AddToCart(int id, string returnUrl)
        {
            var userId = _userManager.GetUserId(HttpContext.User); // Hal-hazırda daxil olmuş istifadəçi ID-sini əldə et

            if (userId == null) // Əgər istifadəçi məlumatı tapılmadısa, səhv qaytarırıq
            {
                return Unauthorized(); // İstifadəçi məlumatı yoxdursa, səlahiyyət səhvi verir
            }

            // Məhsulun olub olmadığını yoxlayaq
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) // Məhsul tapılmadıqda səhv qaytarırıq
            {
                return NotFound("Product not found.");
            }

            // Səbətdə məhsulun olub olmadığını yoxlayırıq
            var existingProductOperation = await _context.ProductOperations
                .FirstOrDefaultAsync(po => po.UserID == userId && po.ProductID == id && po.InCart);

            if (existingProductOperation != null) // Əgər məhsul artıq səbətdə varsa, sayını artırırıq
            {
                existingProductOperation.Count += 1; // Hər dəfə tək məhsul əlavə edirik
            }
            else // Əgər səbətdə yoxdursa, yeni əməliyyat yaradırıq
            {
                var productOperation = new ProductOperation()
                {
                    ProductID = id,
                    UserID = userId,
                    InCart = true,
                    Count = 1, // Başlanğıc olaraq 1 məhsul əlavə edirik
                    IsOrdered = false,
                    IsPending = false,
                    IsCompleted = false,
                    IsDeleted = false
                };

                await _context.ProductOperations.AddAsync(productOperation); // Yeni əməliyyat əlavə edilir
            }

            // Məhsulun stokunu yeniləyirik
            product.Count -= 1;

            // Dəyişiklikləri yadda saxlayırıq
            await _context.SaveChangesAsync();

            // ReturnUrl varsa, ona yönləndiririk
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }



    }
}
