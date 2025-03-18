using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RangShala.Data;
using RangShala.Helpers; // Assuming this contains SetObjectAsJson/GetObjectFromJson
using RangShala.Models;
using RangShala.Services; // Add this for EmailService
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RangShala.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<ApplicationUser> _passwordHasher = new();
        private readonly ILogger<AccountController> _logger;
        private readonly EmailService _emailService; // Added for email functionality

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger, EmailService emailService)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
        }

        // GET: Login page
        public IActionResult Login()
        {
            var user = HttpContext.Session.GetObjectFromJson<ApplicationUser>("User");
            if (user != null)
            {
                _logger.LogInformation($"User {user.Email} is already logged in, redirecting to cart.");
                return RedirectToAction("Index", "Cart");
            }
            return View();
        }

        // GET: Register page
        public IActionResult Register() => View();

        // POST: Register new user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string title, string firstName, string lastName, string email, string mobile, string password, string confirmPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(password) || password != confirmPassword)
                {
                    TempData["Error"] = "Passwords do not match or are empty.";
                    _logger.LogWarning("Registration failed: Password mismatch or empty.");
                    return RedirectToAction("Register");
                }

                if (_context.ApplicationUsers.Any(u => u.Email == email))
                {
                    TempData["Error"] = "Email is already registered.";
                    _logger.LogWarning($"Registration failed: Email {email} already exists.");
                    return RedirectToAction("Register");
                }

                var user = new ApplicationUser
                {
                    Title = title,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Mobile = mobile,
                    Password = _passwordHasher.HashPassword(null, password),
                    EmailVerificationToken = Guid.NewGuid().ToString(),
                    IsEmailVerified = false
                };

                _context.ApplicationUsers.Add(user);
                _context.SaveChanges();

                _logger.LogInformation($"User {email} registered successfully.");
                TempData["SuccessMessage"] = "Registration successful! Please verify your email.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during registration for email {email}.");
                TempData["Error"] = "An error occurred while registering. Please try again.";
                return RedirectToAction("Register");
            }
        }

        // POST: Login user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    TempData["Error"] = "Email and password are required.";
                    _logger.LogWarning("Login attempt with empty email or password.");
                    return RedirectToAction("Login");
                }

                var user = _context.ApplicationUsers.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    TempData["Error"] = "Invalid email or password.";
                    _logger.LogWarning($"Login failed: No user found with email {email}.");
                    return RedirectToAction("Login");
                }

                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(null, user.Password, password);
                if (passwordVerificationResult != PasswordVerificationResult.Success)
                {
                    TempData["Error"] = "Invalid email or password.";
                    _logger.LogWarning($"Login failed: Incorrect password for email {email}.");
                    return RedirectToAction("Login");
                }

                HttpContext.Session.SetObjectAsJson("User", user);
                _logger.LogInformation($"User {email} logged in successfully.");

                TempData["SuccessMessage"] = "Login successful!";
                return RedirectToAction("Index", "Cart");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during login for email {email}.");
                TempData["Error"] = "An error occurred while logging in. Please try again.";
                return RedirectToAction("Login");
            }
        }

        // Logout user
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: Forgot Password page
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Handle Forgot Password request with OTP
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                TempData["Error"] = "Please enter your email.";
                _logger.LogWarning("Forgot password request with empty email.");
                return View();
            }

            var user = _context.ApplicationUsers.FirstOrDefault(u => u.Email == Email);
            if (user == null)
            {
                TempData["Error"] = "No account found with this email.";
                _logger.LogWarning($"Forgot password failed: No user found with email {Email}.");
                return View();
            }

            // Generate OTP
            var otp = new Random().Next(100000, 999999).ToString(); // 6-digit OTP
            var otpRecord = new OtpRecord
            {
                Email = Email,
                Otp = otp,
                CreatedAt = DateTime.Now,
                IsUsed = false
            };
            _context.OtpRecords.Add(otpRecord);
            await _context.SaveChangesAsync();

            // Send OTP via email
            string subject = "RangShala Password Reset OTP";
            string body = $@"
                <h2>Password Reset Request</h2>
                <p>Hello {user.FirstName},</p>
                <p>We received a request to reset your RangShala password. Your OTP is:</p>
                <h3>{otp}</h3>
                <p>Enter this OTP on the reset page to proceed. It’s valid for 10 minutes.</p>
                <p>If you didn’t request this, please ignore this email.</p>
                <p>Best regards,<br/>The RangShala Team</p>";

            try
            {
                await _emailService.SendEmailAsync(Email, subject, body);
                _logger.LogInformation($"OTP sent to {Email} for password reset.");
                TempData["SuccessMessage"] = "An OTP has been sent to your email.";
                return RedirectToAction("ResetPassword", new { email = Email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send OTP email to {Email}.");
                TempData["Error"] = "Failed to send OTP email. Please try again.";
                return View();
            }
        }

        // GET: Reset Password page
        public IActionResult ResetPassword(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        // POST: Handle OTP verification and password reset
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string email, string otp, string newPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otp) || string.IsNullOrEmpty(newPassword))
            {
                TempData["Error"] = "All fields are required.";
                _logger.LogWarning("Reset password attempt with missing fields.");
                ViewBag.Email = email;
                return View();
            }

            var otpRecord = _context.OtpRecords
                .FirstOrDefault(o => o.Email == email && o.Otp == otp && !o.IsUsed && o.CreatedAt > DateTime.Now.AddMinutes(-10));
            if (otpRecord == null)
            {
                TempData["Error"] = "Invalid or expired OTP.";
                _logger.LogWarning($"Reset password failed: Invalid or expired OTP for {email}.");
                ViewBag.Email = email;
                return View();
            }

            var user = _context.ApplicationUsers.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                _logger.LogWarning($"Reset password failed: No user found with email {email}.");
                ViewBag.Email = email;
                return View();
            }

            // Update password
            user.Password = _passwordHasher.HashPassword(null, newPassword);
            otpRecord.IsUsed = true;
            _context.Update(user);
            _context.Update(otpRecord);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Password reset successfully for {email}.");
            TempData["SuccessMessage"] = "Password reset successfully. Please log in.";
            return RedirectToAction("Login");
        }
    }
}