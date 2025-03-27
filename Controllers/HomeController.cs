using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RangShala.Data;
using RangShala.Models;
using RangShala.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RangShala.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _dbContext;
        private readonly EmailService _emailService;
        private readonly GoogleMapsSettings _googleMapsSettings;

        public HomeController(
            IWebHostEnvironment webHostEnvironment,
            ApplicationDbContext dbContext,
            EmailService emailService,
            IOptions<GoogleMapsSettings> googleMapsSettings)
        {
            _webHostEnvironment = webHostEnvironment;
            _dbContext = dbContext;
            _emailService = emailService;
            _googleMapsSettings = googleMapsSettings.Value;
        }

        public IActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                ViewBag.SearchResults = new List<string>();
                ViewBag.Query = "";
                return View();
            }

            var allItems = new List<string>
            {
                "Acrylic Painting", "Oil Painting", "Drawings", "Artist",
                "Anime", "Mandala Art", "Niyati Agravat", "Ayushi Babariya"
            };

            var filteredItems = allItems
                .Where(item => item.ToLower().Contains(query.ToLower()))
                .ToList();

            ViewBag.SearchResults = filteredItems;
            ViewBag.Query = query;

            return View();
        }

        public IActionResult Index()
        {
            ViewBag.ApiKey = _googleMapsSettings.ApiKey;

            var mapData = new MapData
            {
                Latitude = 23.0225,  // Ahmedabad, Gujarat
                Longitude = 72.5714,
                Title = "RangShala Location"
            };

            return View(mapData);
        }

        public IActionResult About()
        {
            ViewBag.Title = "About Us";
            return View("Index");
        }

        public IActionResult Artwork()
        {
            return View();
        }

        public IActionResult Artist()
        {
            return View();
        }

        public IActionResult Bespoke()
        {
            return View();
        }

        public IActionResult Join()
        {
            return View();
        }

        public IActionResult Consult()
        {
            return View();
        }

        public IActionResult AcrylicPaintings()
        {
            var acrylicPaintings = _dbContext.Artworks
                .Where(a => a.Category == "Acrylic Painting")
                .ToList();
            return View(acrylicPaintings);
        }

        [HttpGet]
        public IActionResult GetAcrylicPaintings()
        {
            var acrylicPaintings = _dbContext.Artworks
                .Where(a => a.Category == "Acrylic Painting")
                .ToList();
            return PartialView("_AcrylicPaintingsPartial", acrylicPaintings);
        }

        public IActionResult Drawings()
        {
            return View();
        }

        public IActionResult MandalaArt()
        {
            return View();
        }

        public IActionResult Anime()
        {
            return View();
        }

        public IActionResult OilPainting()
        {
            return View();
        }

        public IActionResult ArtworkGallery()
        {
            return View();
        }

        public IActionResult LoginRegister()
        {
            return View();
        }

        public IActionResult BeSpokeService()
        {
            return View("Bespoke"); // Explicitly specify the view name to match the file
        }

        [HttpPost]
        public async Task<IActionResult> SubmitArtAdvisory(string Name, string CompanyName, string Designation, string Email, string CountryCode, string MobileNo, string Enquiry)
        {
            try
            {
                // Save the form data to the database
                var enquiry = new ArtAdvisoryEnquiry
                {
                    Name = Name,
                    CompanyName = CompanyName,
                    Designation = Designation,
                    Email = Email,
                    CountryCode = CountryCode,
                    MobileNo = MobileNo,
                    Enquiry = Enquiry,
                    SubmissionDate = DateTime.Now
                };

                _dbContext.ArtAdvisoryEnquiries.Add(enquiry);
                await _dbContext.SaveChangesAsync();

                // Prepare the email body with form details
                string subject = "New Art Advisory Enquiry from " + Name;
                string body = $@"
                    <h2>New Art Advisory Enquiry</h2>
                    <p>We have received a new enquiry through the Art Advisory Services form on the Rang Shala website. Below are the details:</p>
                    <ul>
                        <li><strong>Name:</strong> {Name}</li>
                        <li><strong>Company Name:</strong> {CompanyName}</li>
                        <li><strong>Designation:</strong> {Designation}</li>
                        <li><strong>Email:</strong> {Email}</li>
                        <li><strong>Mobile No:</strong> {CountryCode} {MobileNo}</li>
                        <li><strong>Enquiry:</strong> {Enquiry}</li>
                        <li><strong>Submission Date:</strong> {enquiry.SubmissionDate}</li>
                    </ul>
                    <p>Please follow up with the client at your earliest convenience.</p>
                    <p>Best regards,<br/>Rang Shala Team</p>";

                // Send the email to the admin
                await _emailService.SendEmailAsync("ayushibabariya4@gmail.com", subject, body);

                // Send a confirmation email to the user
                string userSubject = "Thank You for Your Enquiry - Rang Shala";
                string userBody = $@"
                    <h2>Hello {Name},</h2>
                    <p>Thank you for reaching out to us through our Art Advisory Services form!</p>
                    <p>We have received your enquiry, and our team will get back to you shortly. Below are the details you submitted:</p>
                    <ul>
                        <li><strong>Name:</strong> {Name}</li>
                        <li><strong>Company Name:</strong> {CompanyName}</li>
                        <li><strong>Designation:</strong> {Designation}</li>
                        <li><strong>Email:</strong> {Email}</li>
                        <li><strong>Mobile No:</strong> {CountryCode} {MobileNo}</li>
                        <li><strong>Enquiry:</strong> {Enquiry}</li>
                        <li><strong>Submission Date:</strong> {enquiry.SubmissionDate}</li>
                    </ul>
                    <p>If you have any further questions, feel free to contact us.</p>
                    <p>Best regards,<br/>The Rang Shala Team</p>";

                await _emailService.SendEmailAsync(Email, userSubject, userBody);

                TempData["SuccessMessage"] = "Your enquiry has been submitted successfully! A confirmation email has been sent to your email address.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "There was an error submitting your enquiry. Please try again later.";
                Console.WriteLine($"Error saving enquiry or sending email: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }

            return RedirectToAction("BeSpokeService");
        }

        public IActionResult Ayushi()
        {
            return View();
        }

        public IActionResult Niyati()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> JoinUsSubmit(
            IFormFile artwork,
            string name,
            DateTime dob,
            string address,
            string address2,
            string email,
            string phone,
            string occupation,
            bool terms)
        {
            Console.WriteLine($"Terms value received: {terms}");

            if (!terms)
            {
                TempData["Error"] = "You must agree to the terms and conditions.";
                return RedirectToAction("Join");
            }

            if (artwork != null && artwork.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + artwork.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await artwork.CopyToAsync(stream);
                }

                var application = new JoinApplication
                {
                    Name = name,
                    DateOfBirth = dob,
                    Address = address,
                    Address2 = address2,
                    Email = email,
                    Phone = phone,
                    Occupation = occupation,
                    ArtworkFilePath = $"/Uploads/{uniqueFileName}",
                    SubmissionDate = DateTime.Now
                };

                _dbContext.JoinApplications.Add(application);
                await _dbContext.SaveChangesAsync();

                string subject = "Thank You for Joining Rang Shala!";
                string body = $@"
                <h2>Hello {name},</h2>
                <p>Thank you for submitting your application to join Rang Shala!</p>
                <p>We have received your details and artwork. Our team will review your submission and get back to you soon.</p>
                <p><strong>Submitted Details:</strong></p>
                <ul>
                    <li>Name: {name}</li>
                    <li>Email: {email}</li>
                    <li>Phone: {phone}</li>
                    <li>Artwork: <a href='{Request.Scheme}://{Request.Host}{application.ArtworkFilePath}'>View Artwork</a></li>
                </ul>
                <p>Best regards,<br/>The Rang Shala Team</p>";

                try
                {
                    await _emailService.SendEmailAsync(email, subject, body);
                    TempData["Message"] = "Your application has been submitted successfully, and a confirmation email has been sent!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Your application was submitted, but we couldn’t send the confirmation email. Please contact support.";
                    Console.WriteLine($"Email sending failed: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                }
            }
            else
            {
                TempData["Error"] = "Please upload an artwork.";
            }

            return RedirectToAction("Join");
        }
    }
}