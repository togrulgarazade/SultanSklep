using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using SultanSklep.DataAccessLayer;
using SultanSklep.Models;

namespace SultanSklep.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var appUser = HttpContext.User;

            // FullName məlumatını yoxlayırıq
            var fullNameClaim = appUser.Claims.FirstOrDefault(c => c.Type == "FullName");

            if (fullNameClaim == null)
            {
                // FullName yoxdursa, istifadəçi məlumat bazasından çəkmək
                var userName = User.Identity.Name; // Buradan UserName əldə edilir
                var user = _context.Users.FirstOrDefault(u => u.UserName == userName);

                if (user != null)
                {
                    // FullName əlavə olunur
                    var claimsIdentity = (ClaimsIdentity)User.Identity;
                    claimsIdentity.AddClaim(new Claim("FullName", user.FullName));
                }
            }

            return View();
        }
    }
}
