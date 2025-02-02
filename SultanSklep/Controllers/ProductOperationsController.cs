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
using System.IO;
using Stripe;

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
            /*var userId = User.Identity.Name;*/ // Daxil olan istifadəçinin adı və ya ID
            var userId = _userManager.GetUserId(HttpContext.User);
            // İstifadəçinin səbətindəki məhsulları çəkirik
            var productOperations = await _context.ProductOperations
                .Where(po => po.UserID == userId && po.InCart == true)
                .Include(po => po.Product) // Məhsul məlumatlarını da əlavə et
                .ToListAsync();

            // Əgər səbət boşdursa
            if (!productOperations.Any())
            {
                return View(new ProductViewModel
                {                    ProductsInCart = new List<ProductOperation>() // Boş səbət
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
        public async Task<IActionResult> AddToCart(int productId, int count, string returnUrl)
        {
            Console.WriteLine(productId);
            Console.WriteLine(count);
            var userId = _userManager.GetUserId(HttpContext.User); // Hal-hazırda daxil olmuş istifadəçi ID-sini əldə et

            if (userId == null) // Əgər istifadəçi məlumatı tapılmadısa, səhv qaytarırıq
            {
                return Unauthorized(); // İstifadəçi məlumatı yoxdursa, səlahiyyət səhvi verir
            }

            // Məhsulun olub olmadığını yoxlayaq
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null) // Məhsul tapılmadıqda səhv qaytarırıq
            {
                return NotFound("Product not found.");
            }

            // Səbətdə məhsulun olub olmadığını yoxlayırıq
            var existingProductOperation = await _context.ProductOperations
                .FirstOrDefaultAsync(po => po.UserID == userId && po.ProductID == productId && po.InCart);

            if (existingProductOperation != null) // Əgər məhsul artıq səbətdə varsa, sayını artırırıq
            {
                existingProductOperation.Count += 1; // Hər dəfə tək məhsul əlavə edirik
            }
            else // Əgər səbətdə yoxdursa, yeni əməliyyat yaradırıq
            {
                var productOperation = new ProductOperation()
                {
                    ProductID = productId,
                    UserID = userId,
                    InCart = true,
                    Count = count, // Başlanğıc olaraq 1 məhsul əlavə edirik
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

        [Authorize]
        public async Task<IActionResult> Order()
        {
            var userId = _userManager.GetUserId(HttpContext.User); // Get the current user's ID.

            if (userId == null)
            {
                return Unauthorized("User not logged in."); // Ensure the user is authorized.
            }

            // Retrieve all products in the cart for the current user.
            var cartItems = await _context.ProductOperations
                .Where(po => po.UserID == userId && po.InCart)
                .Include(po => po.Product)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return BadRequest("Your cart is empty."); // Ensure the cart is not empty.
            }



            // Update the cart items to mark them as ordered.
            foreach (var cartItem in cartItems)
            {
                cartItem.InCart = false;
                cartItem.IsOrdered = true;
            }

            // Save changes to the database.
            await _context.SaveChangesAsync();

            // Redirect to a confirmation page or order summary.
            return RedirectToAction("OrderConfirmation");
        }



        [HttpPost]
        public async Task<IActionResult> CreateCheckoutSession2()
        {
            try
            {
                // Daxil olmuş istifadəçinin ID-sini əldə edin
                var userId = _userManager.GetUserId(HttpContext.User);

                if (userId == null)
                {
                    return Json(new { error = "User not logged in." }); // JSON cavabı qaytarılır
                }

                // Səbətdəki məhsulları əldə edin
                var cartItems = await _context.ProductOperations
                    .Where(po => po.UserID == userId && po.InCart)
                    .Include(po => po.Product)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    return Json(new { error = "Your cart is empty." }); // JSON cavabı qaytarılır
                }

                // Stripe sessiyası üçün məhsul məlumatlarını əlavə edin
                var lineItems = new List<Stripe.Checkout.SessionLineItemOptions>();
                foreach (var item in cartItems)
                {
                    lineItems.Add(new Stripe.Checkout.SessionLineItemOptions
                    {
                        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Product.Price * 100), // Məbləği sentə çevirmək üçün
                            Currency = "azn", // Valyuta
                            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.ProductName, // Məhsulun adı
                            },
                        },
                        Quantity = item.Count, // Məhsul sayı
                    });
                }

                // Stripe sessiyasını yaradın
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" }, // Ödəniş metodları
                    LineItems = lineItems,
                    Mode = "payment", // Ödəniş rejimi
                    SuccessUrl = "https://localhost:5001/ProductOperations/Order2", // Uğurlu ödənişdən sonra
                    CancelUrl = "https://localhost:5001/ProductOperations/Cart", // Ləğv olunduqda
                };

                var service = new Stripe.Checkout.SessionService();
                var session = service.Create(options);

                if (session == null || string.IsNullOrEmpty(session.Id))
                {
                    return Json(new { error = "Failed to create Stripe session." }); // JSON cavabı qaytarılır
                }

                return Json(new { id = session.Id }); // Uğurlu cavab qaytarılır
            }
            catch (Exception ex)
            {
                // Xəta baş verərsə JSON formatında cavab qaytarılır
                Console.WriteLine($"Error creating Stripe session: {ex.Message}");
                return Json(new { error = $"An error occurred: {ex.Message}" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateCheckoutSession()
        {
            try
            {
                // Get the logged-in user's ID
                var userId = _userManager.GetUserId(HttpContext.User);

                if (userId == null)
                {
                    return Json(new { error = "User not logged in." }); // Return JSON error response
                }

                // Retrieve cart items for the user
                var cartItems = await _context.ProductOperations
                    .Where(po => po.UserID == userId && po.InCart)
                    .Include(po => po.Product)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    return Json(new { error = "Your cart is empty." }); // Return JSON error response
                }

                // Prepare line items for Stripe Checkout
                var lineItems = new List<Stripe.Checkout.SessionLineItemOptions>();
                foreach (var item in cartItems)
                {
                    lineItems.Add(new Stripe.Checkout.SessionLineItemOptions
                    {
                        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Product.Price * 100), // Convert price to cents
                            Currency = "usd", // Specify the currency
                            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.ProductName, // Product name
                            },
                        },
                        Quantity = item.Count, // Product quantity
                    });
                }

                // Create a Stripe Checkout session
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = lineItems,
                    Mode = "payment",
                    SuccessUrl = "https://localhost:5001/ProductOperations/CreateCheckoutSession?status=success", // Redirect on success
                    CancelUrl = "https://localhost:5001/ProductOperations/CreateCheckoutSession?status=cancel",  // Redirect on cancel
                };

                var service = new Stripe.Checkout.SessionService();
                var session = service.Create(options);

                if (session == null || string.IsNullOrEmpty(session.Id))
                {
                    return Json(new { error = "Failed to create Stripe session." }); // Return JSON error response
                }

                // Check the payment status after the session is created
                if (Request.Query["status"] == "success")
                {
                    // Update the database: mark items as ordered
                    foreach (var item in cartItems)
                    {
                        item.InCart = false;
                        item.IsOrdered = true;
                    }

                    await _context.SaveChangesAsync();

                    Console.WriteLine("Payment successful! Cart items have been marked as ordered.");
                    return Json(new { message = "Payment successful!" }); // Return success response
                }
                else if (Request.Query["status"] == "cancel")
                {
                    Console.WriteLine("Payment was canceled by the user.");
                    return Json(new { message = "Payment canceled." }); // Return cancel response
                }

                return Json(new { id = session.Id }); // Return session ID for redirect
            }
            catch (Exception ex)
            {
                // Log and return any errors that occur
                Console.WriteLine($"Error during checkout session creation: {ex.Message}");
                return Json(new { error = $"An error occurred: {ex.Message}" });
            }
        }


        //[HttpPost]
        //[Route("stripe/webhook")]
        //public async Task<IActionResult> StripeWebhook()
        //{
        //    var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        //    try
        //    {
        //        var stripeEvent = EventUtility.ConstructEvent(
        //            json,
        //            Request.Headers["Stripe-Signature"],
        //            "your_stripe_webhook_secret" // Replace with your actual Stripe webhook secret
        //        );

        //        if (stripeEvent.Type == Events.CheckoutSessionCompleted)
        //        {
        //            var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

        //            if (session != null)
        //            {
        //                // Handle successful payment
        //                var sessionId = session.Id;
        //                var customerEmail = session.CustomerEmail;

        //                // Fetch cart items using session metadata
        //                var cartItems = await _context.ProductOperations
        //                    .Where(po => po.SessionId == sessionId) // Store SessionId in DB when creating the session
        //                    .ToListAsync();

        //                if (cartItems != null)
        //                {
        //                    foreach (var item in cartItems)
        //                    {
        //                        item.InCart = false;
        //                        item.IsOrdered = true;
        //                    }

        //                    await _context.SaveChangesAsync();
        //                    Console.WriteLine($"Payment successful for session {sessionId}. Items marked as ordered.");
        //                }
        //            }
        //        }
        //        else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
        //        {
        //            Console.WriteLine("Payment failed.");
        //            // Handle payment failure if necessary
        //        }

        //        return Ok();
        //    }
        //    catch (StripeException ex)
        //    {
        //        Console.WriteLine($"Stripe webhook error: {ex.Message}");
        //        return BadRequest();
        //    }
        //}




    }
}
