using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SultanSklep.DataAccessLayer;
using SultanSklep.Models;
using SultanSklep.Utilities;
using SultanSklep.ViewModels.AccountViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SultanSklep.Controllers
{
    public class AccountController : Controller
    {
        private AppDbContext _context { get; }
        private SignInManager<ApplicationUser> _signInManager { get; }
        private UserManager<ApplicationUser> _userManager { get; }
        private RoleManager<IdentityRole> _roleManager { get; }

        public AccountController(AppDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {

            if (!ModelState.IsValid) return View(registerViewModel);

            ApplicationUser newUser = new ApplicationUser
            {
                FullName = registerViewModel.Name + registerViewModel.Surname,
                Name = registerViewModel.Name,
                Surname = registerViewModel.Surname,
                UserName = registerViewModel.UserName,
                Email = registerViewModel.Email,

                EmailConfirmed = true

            };

            var identityResult = await _userManager.CreateAsync(newUser, registerViewModel.Password);
            await _userManager.AddToRoleAsync(newUser, UserRoles.Admin.ToString());




            return RedirectToAction("Login", "Account");

        }




        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
                return View(login);

            var result = await _signInManager.PasswordSignInAsync(
                login.UserName, login.Password,
                isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError(string.Empty, "Something is wrong...");
            return View(login);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        #region for create roles

        //public async Task CreateRole()
        //{
        //    foreach (var role in Enum.GetValues(typeof(UserRoles)))
        //    {
        //        if (!await _roleManager.RoleExistsAsync(role.ToString()))
        //        {
        //            await _roleManager.CreateAsync(new IdentityRole { Name = role.ToString() });
        //        }
        //    }
        //}

        #endregion
    }
}
