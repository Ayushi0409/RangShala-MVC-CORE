using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RangShala.Data;
using RangShala.Models;
using RangShala.Services; // For EmailService and WeatherService
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ApplicationDbContext _dbContext;
    private readonly EmailService _emailService;
    private readonly WeatherService _weatherService; // Added WeatherService

    public HomeController(
        IWebHostEnvironment webHostEnvironment,
        ApplicationDbContext dbContext,
        EmailService emailService,
        WeatherService weatherService) // Added WeatherService to constructor
    {
        _webHostEnvironment = webHostEnvironment;
        _dbContext = dbContext;
        _emailService = emailService;
        _weatherService = weatherService;
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

    public async Task<IActionResult> Index()
    {
        var weatherData = await _weatherService.GetWeatherAsync("Delhi"); // Fetch weather for Delhi
        ViewBag.Weather = weatherData; // Pass raw JSON to view (parse it if needed)
        return View();
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
        return View();
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

    public IActionResult BeSpokeservice()
    {
        return View();
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

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(artwork.FileName);
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