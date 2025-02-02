using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SultanSklep.Areas.AdminArea.Controllers
{ 
    [Area("AdminArea")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> NewOrders()
        {
            return View();
        }

        public async Task<IActionResult> PendingOrders()
        {
            return View();
        }

        public async Task<IActionResult> SendedOrders()
        {
            return View();
        }

        public async Task<IActionResult> CompletedOrders()
        {
            return View();
        }
    }

}