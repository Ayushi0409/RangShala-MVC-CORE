using Microsoft.AspNetCore.Mvc;
using RangShala.Data;
using RangShala.Helpers;
using RangShala.Models;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using Microsoft.Extensions.Options;

namespace RangShala.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RazorpaySettings _razorpaySettings;

        public CheckoutController(ApplicationDbContext context, IOptions<RazorpaySettings> razorpaySettings)
        {
            _context = context;
            _razorpaySettings = razorpaySettings.Value ?? throw new ArgumentNullException(nameof(razorpaySettings));
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
                Email = user.Email ?? string.Empty
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

            var cartItems = _context.CartItems.Where(c => c.UserId == user.Id).ToList();
            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty. Add items to proceed.";
                return RedirectToAction("Index", "Cart");
            }

            decimal totalAmount = cartItems.Sum(item => item.Price * item.Quantity) * 100;

            var client = new RazorpayClient(_razorpaySettings.KeyId, _razorpaySettings.KeySecret);
            var orderOptions = new Dictionary<string, object>
            {
                { "amount", totalAmount },
                { "currency", "INR" },
                { "receipt", $"order_rcptid_{user.Id}_{DateTime.Now.Ticks}" },
                { "payment_capture", 1 }
            };

            var order = client.Order.Create(orderOptions);
            var model = new PaymentModel
            {
                OrderId = order["id"]?.ToString(),
                Amount = totalAmount,
                Currency = "INR",
                KeyId = _razorpaySettings.KeyId,
                Email = user.Email,
                Name = $"{user.FirstName} {user.LastName}",
                Contact = user.Mobile
            };

            return View(model);
        }

        // POST: /Checkout/VerifyPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyPayment(string razorpay_payment_id, string razorpay_order_id, string razorpay_signature)
        {
            var user = HttpContext.Session.GetObjectFromJson<ApplicationUser>("User");
            if (user == null)
            {
                Console.WriteLine("User not found in session");
                return RedirectToAction("Login", "Account");
            }
            Console.WriteLine($"User ID: {user.Id}");

            var attributes = new Dictionary<string, string>
    {
        { "razorpay_payment_id", razorpay_payment_id },
        { "razorpay_order_id", razorpay_order_id },
        { "razorpay_signature", razorpay_signature }
    };

            try
            {
                Console.WriteLine("Verifying payment signature...");
                Razorpay.Api.Utils.verifyPaymentSignature(attributes);
                Console.WriteLine("Payment signature verified successfully");

                var payment = new RangShala.Models.Payment
                {
                    UserId = user.Id,
                    RazorpayOrderId = razorpay_order_id,
                    RazorpayPaymentId = razorpay_payment_id,
                    RazorpaySignature = razorpay_signature,
                    Amount = _context.CartItems.Where(c => c.UserId == user.Id).Sum(c => c.Price * c.Quantity) * 100,
                    Currency = "INR",
                    Email = user.Email,
                    Name = $"{user.FirstName} {user.LastName}",
                    Contact = user.Mobile,
                    PaymentDate = DateTime.Now,
                    IsSuccessful = true
                };
                _context.Payments.Add(payment);
                Console.WriteLine($"Payment saved: PaymentId={payment.RazorpayPaymentId}");

                var cartItems = _context.CartItems.Where(c => c.UserId == user.Id).ToList();
                Console.WriteLine($"Found {cartItems.Count} cart items for user {user.Id}");

                try
                {
                    _context.CartItems.RemoveRange(cartItems);
                    _context.SaveChanges();
                    Console.WriteLine("Cart cleared and changes saved");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to clear cart: {ex.Message}");
                    throw;
                }

                TempData["SuccessMessage"] = "Payment successful! Order placed.";
                return RedirectToAction("Confirmation");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Payment verification failed: {ex.Message}");
                TempData["Error"] = "Payment verification failed: " + ex.Message;
                return RedirectToAction("Payment");
            }
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