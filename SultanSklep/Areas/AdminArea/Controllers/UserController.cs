using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SultanSklep.DataAccessLayer;
using SultanSklep.Models;
using SultanSklep.ViewModels.AccountViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SultanSklep.Areas.AdminArea.Controllers
{

    [Area("AdminArea")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {

        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private AppDbContext _context { get; }

        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);

            var userViewModels = new UserViewModel()
            {
                ApplicationUsers = users
            };


            return View(userViewModels);
        }
    }
}
