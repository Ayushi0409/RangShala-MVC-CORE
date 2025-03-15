using Microsoft.AspNetCore.Mvc;
using RangShala.Data;
using RangShala.Helpers;
using RangShala.Models;
using Microsoft.EntityFrameworkCore;

namespace RangShala.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Checkout/Checkout
        public IActionResult Checkout()
        {
            var user = HttpContext.Session.GetObjectFromJson<ApplicationUser>("User");
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = _context.CartItems
                .Where(c => c.UserId == user.Id)
                .ToList();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty. Add items to proceed to checkout.";
                return RedirectToAction("Index", "Cart");
            }

            var model = new CheckoutModel
            {
                Email = user?.Email ?? string.Empty
            };

            return View(model);
        }

        // POST: /Checkout/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(CheckoutModel model)
        {
            var user = HttpContext.Session.GetObjectFromJson<ApplicationUser>("User");
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var dbUser = _context.ApplicationUsers.Find(user.Id);
            if (dbUser == null)
            {
                TempData["Error"] = "User not found in the database. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var billingInfo = new BillingInfo
            {
                UserId = dbUser.Id,
                Country = model.Country,
                Address = model.Address,
                City = model.City,
                State = model.State,
                PinCode = model.PinCode,
                Email = model.Email,
                Phone = model.Phone
            };

            try
            {
                _context.BillingInfos.Add(billingInfo);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Failed to save billing information. Error: {ex.InnerException?.Message ?? ex.Message}";
                return View(model);
            }

            TempData["BillingInfo"] = Newtonsoft.Json.JsonConvert.SerializeObject(billingInfo);
            return RedirectToAction("Payment");
        }

        // GET: /Checkout/Payment
        public IActionResult Payment()
        {
            var user = HttpContext.Session.GetObjectFromJson<ApplicationUser>("User");
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new PaymentModel();
            return View(model);
        }

        // POST: /Checkout/Payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Payment(PaymentModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = HttpContext.Session.GetObjectFromJson<ApplicationUser>("User");
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = _context.CartItems.Where(c => c.UserId == user.Id).ToList();
            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty. Add items to proceed.";
                return RedirectToAction("Index", "Cart");
            }

            // Simulate payment processing
            Console.WriteLine($"Simulated Payment - Name: {model.NameOnCard}, Card: {model.CardNumber}, Expiry: {model.ExpirationMonth}/{model.ExpirationYear}, CVV: {model.SecurityCode}");

            // Clear the cart
            _context.CartItems.RemoveRange(cartItems);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Payment successful! Order placed.";
            return RedirectToAction("Confirmation");
        }

        // GET: /Checkout/Confirmation
        public IActionResult Confirmation(bool offline = false)
        {
            var user = HttpContext.Session.GetObjectFromJson<ApplicationUser>("User");
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (offline)
            {
                var cartItems = _context.CartItems.Where(c => c.UserId == user.Id).ToList();
                if (cartItems.Any())
                {
                    _context.CartItems.RemoveRange(cartItems);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Offline payment initiated! Order placed successfully.";
                }
            }

            return View();
        }
    }
}