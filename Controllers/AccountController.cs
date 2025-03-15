using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RangShala.Data;
using RangShala.Helpers;
using RangShala.Models;
using System;
using System.Linq;

namespace RangShala.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<ApplicationUser> _passwordHasher = new();
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

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

        public IActionResult Register() => View();

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
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}