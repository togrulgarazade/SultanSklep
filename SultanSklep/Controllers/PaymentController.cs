using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stripe.Checkout;

namespace SultanSklep.Controllers
{
    public class PaymentController : Controller
    {
        // Checkout səhifəsini göstərən aksiya
        public IActionResult Checkout()
        {
            return View();
        }

        // Stripe Checkout Session yaradan aksiya
        [HttpPost]
        public IActionResult CreateCheckoutSession()
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = 5000, // Məbləğ sentlə (məsələn, 50.00 AZN üçün 5000)
                            Currency = "azn",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Sifariş",
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = "https://localhost:5001/Payment/Success",
                CancelUrl = "https://localhost:5001/Payment/Cancel",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Json(new { id = session.Id });
        }

        // Ödəniş uğurlu olduqda yönləndirilən aksiya
        public IActionResult Success()
        {
            // Burada IsOrdered = true etmək üçün database yenilənə bilər
            return View();
        }

        // Ödəniş ləğv olunduqda yönləndirilən aksiya
        public IActionResult Cancel()
        {
            return View();
        }
    }
}
