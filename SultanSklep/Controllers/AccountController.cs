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


        public IActionResult Authentication()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AccountViewModel registerViewModel)
        {

            if (!ModelState.IsValid) return View("Authentication", registerViewModel);

            ApplicationUser newUser = new ApplicationUser
            {
                FullName = registerViewModel.Name + " " + registerViewModel.Surname,
                Name = registerViewModel.Name,
                Surname = registerViewModel.Surname,
                Email = registerViewModel.Email,
                UserName = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 12), // 12 rəqəmli GUID
                PhoneNumber = registerViewModel.PhoneNumber,
            };

            var identityResult = await _userManager.CreateAsync(newUser, registerViewModel.Password);


            if (identityResult.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { token, email = registerViewModel.Email }, Request.Scheme);


                
                var msgArea =
                    $"<body style=\"height: 100% !important;margin: 0 !important;padding: 0 !important;width: 100% !important;background-color: #f4f4f4; margin: 0 !important; padding: 0 !important;\"><div style=\"display: none; font-size: 1px; color: #fefefe; line-height: 1px; font-family: \'Lato\', Helvetica, Arial, sans-serif; max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden;\"> This is a message to contact you. </div><table style=\"border-collapse: collapse !important;\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"><tr><td bgcolor=\"#C8102E\" align=\"center\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\"><tr><td align=\"center\" valign=\"top\" style=\"padding: 40px 10px 40px 10px;\"> </td></tr></table></td></tr><tr><td bgcolor=\"#C8102E\" align=\"center\" style=\"padding: 0px 10px 0px 10px;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;border-collapse: collapse !important;\"><tr><td bgcolor=\"#ffffff\" align=\"center\" valign=\"top\" style=\"padding: 40px 20px 20px 20px; border-radius: 4px 4px 0px 0px; color: #111111; font-family: \'Lato\', Helvetica, Arial, sans-serif; font-size: 48px; font-weight: 400; letter-spacing: 4px; line-height: 48px;\"><h1 style=\"font-size: 48px; font-weight: 400; margin: 2;\">Sultan Sklep Mięsny</h1> <img src=\"https://i.postimg.cc/xTwxKBqC/sultan-no-background.png\" width=\"125\" height=\"120\" style=\"border: 0;height: auto; line-height: 100%;outline: none; text-decoration: none; display: block; border: 0px;\" /></td></tr></table></td></tr><tr><td bgcolor=\"#f4f4f4\" align=\"center\" style=\"padding: 0px 10px 0px 10px;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;border-collapse: collapse !important;\"><tr><td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 20px 30px 40px 30px; color: #666666; font-family: \'Lato\', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\"><p style=\"margin: 0; text-align: center;\">Thank you for registering on our site. Activate your account by clicking the link below.</p></td></tr><tr><td bgcolor=\"#ffffff\" align=\"left\"><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"border-collapse: collapse !important;\"><tr><td bgcolor=\"#ffffff\" align=\"center\" style=\"padding: 20px 30px 60px 30px;\"><table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"border-collapse: collapse !important;\"><tr><td align=\"center\" style=\"border-radius: 3px;\" bgcolor=\"#343434\"><a href=\"{confirmationLink}\" target=\"_blank\" style=\"font-size: 20px; font-family: Helvetica, Arial, sans-serif; color: #ffffff; text-decoration: none; color: #ffffff; text-decoration: none; padding: 15px 25px; border-radius: 2px; border: 1px solid #C8102E; display: inline-block;background: #c8102e;\">Confirmation</a></td></tr></table></td></tr></table></td></tr><tr><td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 0px 30px 0px 30px; color: #666666; font-family: \'Lato\', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\"><p style=\"margin: 0; text-align: center;\">If an error occurs during verification, please contact the admin!</p></td></tr><tr><td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 20px 30px 20px 30px; color: #666666; font-family: \'Lato\', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\"><p style=\"margin: 0; text-align: center;\"><a href=\"mailto:nesib7474@gmail.com\" target=\"_blank\" style=\"color: #C8102E;\">nesib7474@gmail.com</a></p></td></tr><tr><td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 0px 30px 20px 30px; color: #666666; font-family: \'Lato\', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\"><p style=\"margin: 0; text-align: center;\">Thank you very much for your attention!</p></td></tr><tr><td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 0px 30px 40px 30px; border-radius: 0px 0px 4px 4px; color: #666666; font-family: \'Lato\', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\"><p style=\"margin: 0; text-align: center;\">Sincerely: <br>Sultan Sklep Mięsny ©</p></td></tr></table></td></tr><tr><td bgcolor=\"#f4f4f4\" align=\"center\" style=\"padding: 0px 10px 0px 10px;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;border-collapse: collapse !important;\"><tr><td bgcolor=\"#f4f4f4\" align=\"left\" style=\"padding: 0px 30px 30px 30px; color: #666666; font-family: \'Lato\', Helvetica, Arial, sans-serif; font-size: 14px; font-weight: 400; line-height: 18px;\"> <br><p style=\"margin: 0; text-align: center;\">© 2025 All Rights Reserved | Created by: <a href=\"https://wedevloop.com\" target=\"_blank\" style=\"color: #111111; font-weight: 700;\"> WeDevloop</a>.</p></td></tr></table></td></tr></table></body>";

                var subject = "Sultan Sklep Mięsny - Account Verification";

                bool emailResponse = Helper.SendEmail(registerViewModel.Email, msgArea, subject);


                if (emailResponse)
                {
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Admin.ToString());
                    return RedirectToAction("ConfirmedEmail", "Account");
                }
            }
            else
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("Authentication", registerViewModel);
            }


            return RedirectToAction("Authentication", "Account");

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountViewModel loginViewModel, string ReturnUrl)
        {

            if (!ModelState.IsValid) return View("Authentication", loginViewModel);
            ApplicationUser user = await _userManager.FindByEmailAsync(loginViewModel.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email və ya şifrə yanlışdır !");
                return View("Authentication", loginViewModel);
            }

            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Zəhmət olmasa emailinizi təsdiqləyin! Təsdiq mesajı emailə göndərilmişdir !");
                return View("Authentication", loginViewModel);
            }

            var signInResult =
                await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe,
                    true);
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Zəhmət olmasa bir neçə dəqiqə gözləyin !");
                return View("Authentication", loginViewModel);
            }


            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Email və ya şifrə yanlışdır !");
                return View("Authentication", loginViewModel);
            }


            if (ReturnUrl != null)
            {

                return Redirect(ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout(string ReturnUrl)
        {
            await _signInManager.SignOutAsync();

            if (ReturnUrl != null)
            {

                return Redirect(ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        #region Confirmation user email on register

        //Registration Success
        public ActionResult ConfirmedEmail()
        {
            return View();
        }

        //Confirmation Success
        public ActionResult ConfirmationEmail()
        {
            return View();
        }

        //Confirm Email Operation

        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? "ConfirmationEmail" : "Error");
        }

        //Confirm Email Operation - End

        #endregion


        #region Forget password and reset password


        //Reset password
        public IActionResult ResetPass(string token, string email)
        {
            if (token == null && email == null)
            {
                ModelState.AddModelError("", "Axtardığınız email ilə bağlanmış hesab yoxdur ! ");
            }

            return View();
        }

        //Reset password operation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPass(ResetPassViewModel reset)
        {
            if (!ModelState.IsValid) return View(reset);

            var user = await _userManager.FindByEmailAsync(reset.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Email ilə bağlanmış hesab tapılmadı !  Doğru emaili daxil etdiyinizdən əmin olun !");
                return View(user);
            }



            var result = await _userManager.ResetPasswordAsync(user, reset.Token, reset.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    return View();
                }
            }
            return RedirectToAction("ResetSuccess", "Account");

        }
        //Reset password operation - End

        //Reset password succesful
        public IActionResult ResetSuccess()
        {
            return View();
        }

        //Forget password
        public IActionResult ForgetPass()
        {
            return View();
        }

        //Forget password operation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPass(ForgetPassViewModel forget)
        {
            if (!ModelState.IsValid) return View(forget);

            var user = await _userManager.FindByEmailAsync(forget.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Email ilə bağlanmış hesab tapılmadı !  Doğru emaili daxil etdiyinizdən əmin olun !");
                return View(user);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            string confirmationLink = Url.Action("ResetPass", "Account", new
            {
                email = user.Email,
                token = token
            }, protocol: HttpContext.Request.Scheme);



            var msg =
                    $"<body style=\"height: 100% !important;margin: 0 !important;padding: 0 !important;width: 100% !important;background-color: #f4f4f4; margin: 0 !important; padding: 0 !important;\"><div style=\"display: none; font-size: 1px; color: #fefefe; line-height: 1px; font-family: \'Lato\', Helvetica, Arial, sans-serif; max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden;\"> This is a message to contact you. </div><table style=\"border-collapse: collapse !important;\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"><tr><td bgcolor=\"#C8102E\" align=\"center\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\"><tr><td align=\"center\" valign=\"top\" style=\"padding: 40px 10px 40px 10px;\"> </td></tr></table></td></tr><tr><td bgcolor=\"#C8102E\" align=\"center\" style=\"padding: 0px 10px 0px 10px;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;border-collapse: collapse !important;\"><tr><td bgcolor=\"#ffffff\" align=\"center\" valign=\"top\" style=\"padding: 40px 20px 20px 20px; border-radius: 4px 4px 0px 0px; color: #111111; font-family: \'Lato\', Helvetica, Arial, sans-serif; font-size: 48px; font-weight: 400; letter-spacing: 4px; line-height: 48px;\"><h1 style=\"font-size: 48px; font-weight: 400; margin: 2;\">Sultan Sklep Mięsny</h1> <img src=\"https://i.postimg.cc/xTwxKBqC/sultan-no-background.png\" width=\"125\" height=\"120\" style=\"border: 0;height: auto; line-height: 100%;outline: none; text-decoration: none; display: block; border: 0px;\" /></td></tr></table></td></tr><tr><td bgcolor=\"#f4f4f4\" align=\"center\" style=\"padding: 0px 10px 0px 10px;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;border-collapse: collapse !important;\"><tr><td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 20px 30px 40px 30px; color: #666666; font-family: \'Lato\', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\"><p style=\"margin: 0; text-align: center;\">Thank you for contacting us. Please confirm that you are the one who updated your password by clicking the link below!</p></td></tr><tr><td bgcolor=\"#ffffff\" align=\"left\"><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"border-collapse: collapse !important;\"><tr><td bgcolor=\"#ffffff\" align=\"center\" style=\"padding: 20px 30px 60px 30px;\"><table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"border-collapse: collapse !important;\"><tr><td align=\"center\" style=\"border-radius: 3px;\" bgcolor=\"#343434\"><a href=\"{confirmationLink}\" target=\"_blank\" style=\"font-size: 20px; font-family: Helvetica, Arial, sans-serif; color: #ffffff; text-decoration: none; color: #ffffff; text-decoration: none; padding: 15px 25px; border-radius: 2px; border: 1px solid #C8102E; display: inline-block;background: #c8102e;\">Confirmation</a></td></tr></table></td></tr></table></td></tr><tr><td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 0px 30px 0px 30px; color: #666666; font-family: \'Lato\', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\"><p style=\"margin: 0; text-align: center;\">If an error occurs during verification, please contact the admin!</p></td></tr><tr><td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 20px 30px 20px 30px; color: #666666; font-family: \'Lato\', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\"><p style=\"margin: 0; text-align: center;\"><a href=\"mailto:nesib7474@gmail.com\" target=\"_blank\" style=\"color: #C8102E;\">nesib7474@gmail.com</a></p></td></tr><tr><td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 0px 30px 20px 30px; color: #666666; font-family: \'Lato\', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\"><p style=\"margin: 0; text-align: center;\">Thank you very much for your attention!</p></td></tr><tr><td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 0px 30px 40px 30px; border-radius: 0px 0px 4px 4px; color: #666666; font-family: \'Lato\', Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\"><p style=\"margin: 0; text-align: center;\">Sincerely: <br>Sultan Sklep Mięsny ©</p></td></tr></table></td></tr><tr><td bgcolor=\"#f4f4f4\" align=\"center\" style=\"padding: 0px 10px 0px 10px;\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;border-collapse: collapse !important;\"><tr><td bgcolor=\"#f4f4f4\" align=\"left\" style=\"padding: 0px 30px 30px 30px; color: #666666; font-family: \'Lato\', Helvetica, Arial, sans-serif; font-size: 14px; font-weight: 400; line-height: 18px;\"> <br><p style=\"margin: 0; text-align: center;\">© 2025 All Rights Reserved | Created by: <a href=\"https://wedevloop.com\" target=\"_blank\" style=\"color: #111111; font-weight: 700;\"> WeDevloop</a>.</p></td></tr></table></td></tr></table></body>";

            var subject = "Sultan Sklep Mięsny - Password update";

            //SendMailHelper sendEmailHelper = new SendMailHelper();
            //bool emailResponse = SendEmail(forget.Email, msg);
            bool emailResponse = Helper.SendEmail(forget.Email, msg, subject);



            if (emailResponse)
            {
                return RedirectToAction("PassVerification", "Account");
            }

            return View();
        }
        //Forget password operation - End

        //Send email for reset forget account password successful
        public IActionResult PassVerification()
        {
            return View();
        }


        #endregion






        #region for create roles

        public async Task CreateRole()
        {
            foreach (var role in Enum.GetValues(typeof(UserRoles)))
            {
                if (!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = role.ToString() });
                }
            }
        }

        #endregion
    }
}
