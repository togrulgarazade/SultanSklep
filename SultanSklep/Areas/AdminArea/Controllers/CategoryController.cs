using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SultanSklep.DataAccessLayer;
using SultanSklep.Models;

namespace SultanSklep.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string categorySearch = null)
        {
            ViewData["SearchedCategory"] = categorySearch;

            var categories = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(categorySearch))
            {
                categories = categories.Where(c => c.Name.Trim().ToLower().Contains(categorySearch.Trim().ToLower()) && !c.IsDeleted);
            }

            return View(await categories.Where(c => !c.IsDeleted).ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid) return View(category);

            category.IsDeleted = false;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (!ModelState.IsValid) return View(category);

            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (existingCategory == null) return NotFound();

            existingCategory.Name = category.Name;

            _context.Categories.Update(existingCategory);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            category.IsDeleted = true; // Perform soft delete
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
