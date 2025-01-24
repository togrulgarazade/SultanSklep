using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SultanSklep.DataAccessLayer;
using SultanSklep.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SultanSklep.Controllers
{
    public class ProductOperationsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductOperationsController(AppDbContext context)
        {
            _context = context;
        }

        // Səbətə məhsul əlavə etmək
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in.");
            }

            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null || product.Count < quantity)
            {
                return BadRequest("Product is out of stock.");
            }

            var productOperation = new ProductOperation
            {
                UserID = userId,
                ProductID = productId,
                InCart = true,
                IsOrdered = false,
                IsPending = false,
                IsCompleted = false,
                IsDeleted = false,
                Count = quantity // Say məlumatını əlavə edin
            };

            _context.ProductOperations.Add(productOperation);
            _context.SaveChanges();

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in.");
            }

            var productOperation = _context.ProductOperations
                .FirstOrDefault(po => po.Id == id && po.UserID == userId && po.InCart);

            if (productOperation == null)
            {
                return NotFound("Item not found in the cart.");
            }

            productOperation.InCart = false;

            _context.SaveChanges();

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult UpdateCart(Dictionary<int, int> quantities)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in.");
            }

            var cartItems = _context.ProductOperations
                .Where(po => po.UserID == userId && po.InCart && !po.IsDeleted)
                .ToList();

            foreach (var item in cartItems)
            {
                if (quantities.ContainsKey(item.Id))
                {
                    item.Count = quantities[item.Id];  // Sayı yeniləyirik
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Cart");
        }

        // Proceed to Order metodunu yazırıq
        public IActionResult ProceedToOrder()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in.");
            }

            // İstifadəçinin saxlanmış ünvanlarını əldə et
            var addresses = _context.Addresses
                .Where(a => a.UserID == userId)
                .ToList();

            // Əgər istifadəçinin ünvanı yoxdursa, AddAddress səhifəsinə yönləndiririk
            if (addresses.Count == 0)
            {
                return RedirectToAction("AddAddress");
            }

            // Əgər ünvan varsa, SelectAddress səhifəsinə yönləndiririk
            return RedirectToAction("SelectAddress");
        }



        // Səbəti göstərmək
        public IActionResult Cart()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in.");
            }

            var cartItems = _context.ProductOperations
                .Where(po => po.UserID == userId && po.InCart && !po.IsDeleted)
                .Include(po => po.Product) // Məhsul məlumatlarını əlavə edin
                .ToList();

            return View(cartItems);
        }


        // Sifarişi təsdiqləmək
        [HttpPost]
        public IActionResult ConfirmOrder(int addressId, int productOperationId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in.");
            }

            // ProductOperation-u tapırıq
            var productOperation = _context.ProductOperations
                .FirstOrDefault(po => po.Id == productOperationId && po.UserID == userId);

            if (productOperation == null)
            {
                return NotFound("Order not found.");
            }

            // Address-i tapırıq
            var selectedAddress = _context.Addresses
                .FirstOrDefault(a => a.Id == addressId && a.UserID == userId);

            if (selectedAddress == null)
            {
                return NotFound("Selected address not found.");
            }

            // Ünvanı sifarişə bağlayırıq
            productOperation.AddressID = addressId;
            productOperation.IsOrdered = true;
            productOperation.IsPending = true;
            productOperation.InCart = false;

            _context.SaveChanges();

            return RedirectToAction("OrderDetails", new { id = productOperation.Id });
        }





        // Sifariş detalları
        public IActionResult OrderDetails(int id)
        {
            var order = _context.ProductOperations
                .Include(po => po.Product)
                .Include(po => po.Address)
                .FirstOrDefault(po => po.Id == id);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            return View(order);
        }



        public IActionResult SelectAddress()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in.");
            }

            // İstifadəçinin ünvanlarını əldə edirik
            var addresses = _context.Addresses
                .Where(a => a.UserID == userId && !a.AddressLabel.StartsWith("NS"))
                .ToList();

            // Əgər ünvan varsa, onları göndəririk
            if (addresses.Count == 0)
            {
                return RedirectToAction("AddAddress");
            }

            return View(addresses);
        }




        public IActionResult AddAddress()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddAddress(Address address, bool saveAddressLabel)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in.");
            }

            address.UserID = userId;

            if (saveAddressLabel)
            {
                // Əgər istifadəçi AddressLabel təyin etmək istəyirsə, onu istifadə edək
                address.AddressLabel = string.IsNullOrEmpty(address.AddressLabel)
                    ? $"NS{Guid.NewGuid().ToString("N").Substring(0, 8)}"
                    : address.AddressLabel;
            }
            else
            {
                // Əgər istifadəçi AddressLabel saxlamaq istəmirsə, backend-də GUID təyin et
                address.AddressLabel = $"NS{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            }

            _context.Addresses.Add(address);
            _context.SaveChanges();

            // Ünvanı əlavə etdikdən sonra SelectAddress səhifəsinə yönləndiririk
            return RedirectToAction("SelectAddress");
        }



        [HttpPost]
        public IActionResult AddNewAddress(Address address, bool saveAddressLabel)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in.");
            }

            address.UserID = userId;

            // Əgər Save Address seçilibsə, AddressLabel təyin edilir
            if (saveAddressLabel)
            {
                address.AddressLabel = string.IsNullOrEmpty(address.AddressLabel)
                    ? $"NS{Guid.NewGuid().ToString("N").Substring(0, 8)}"
                    : address.AddressLabel;
            }
            else
            {
                // Əgər Save Address seçilməyibsə, GUID ilə AddressLabel təyin olunur
                address.AddressLabel = $"NS{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            }

            _context.Addresses.Add(address);
            _context.SaveChanges();

            // Yeni ünvan əlavə edildikdən sonra SelectAddress səhifəsinə yönləndiririk
            return RedirectToAction("SelectAddress");
        }


        [HttpPost]
        public IActionResult AddNewAddressAndProceed(Address address, bool saveAddressLabel, int productOperationId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in.");
            }

            address.UserID = userId;

            // Save Address seçilibsə, AddressLabel istifadəçinin verdiyi dəyərə əsaslanır
            if (saveAddressLabel && !string.IsNullOrEmpty(address.AddressLabel))
            {
                address.AddressLabel = address.AddressLabel;
            }
            else
            {
                // Əks halda GUID təyin olunur
                address.AddressLabel = $"NS{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            }

            _context.Addresses.Add(address);
            _context.SaveChanges();

            // Yeni ünvan əlavə edildikdən sonra ConfirmOrder metoduna yönləndirilir
            return RedirectToAction("ConfirmOrder", new { addressId = address.Id, productOperationId });
        }


    }
}
