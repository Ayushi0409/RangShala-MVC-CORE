using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;

public class HomeController : Controller
{
    public IActionResult Search(string query)
    {
        // Perform search logic here. For simplicity, we'll assume a list of items to search through.
        var allItems = new List<string>
        {
            "Acrylic Painting", "Oil Painting", "Drawings", "Artist", "Anime", "Mandala Art"
        };

        // Filter the items based on the query (case-insensitive)
        var filteredItems = allItems.Where(item => item.ToLower().Contains(query.ToLower())).ToList();

        // Pass the search results to the view
        ViewBag.SearchResults = filteredItems;
        ViewBag.Query = query;

        return View();
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {
        ViewBag.Title = "About Us";
        return View("Index"); // This will load the same view as your home page.
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
        // You can fetch painting data dynamically from a database here.
        return View();
    }
    public IActionResult Drawings()
    {
        // You can fetch painting data dynamically from a database here.
        return View();
    }
    public IActionResult MandalaArt()
    {
        // You can fetch painting data dynamically from a database here.
        return View();
    }
    public IActionResult Anime()
    {
        // You can fetch painting data dynamically from a database here.
        return View();
    }
    public IActionResult OilPainting()
    {
        // You can fetch painting data dynamically from a database here.
        return View();
    }
    public IActionResult ArtworkGallery()
    {
        // You can fetch painting data dynamically from a database here.
        return View();
    }
    public IActionResult LoginRegister()
    {
        // You can fetch painting data dynamically from a database here.
        return View();
    }
    public IActionResult BeSpokeservice()
    {
        // You can fetch painting data dynamically from a database here.
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
    private readonly IWebHostEnvironment _webHostEnvironment;

    public HomeController(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpPost]
    public async Task<IActionResult> JoinUsSubmit(IFormFile artwork)
    {
        if (artwork != null && artwork.Length > 0)
        {
            // Get the "Uploads" folder path
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");

            // Ensure directory exists
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Get file path
            string filePath = Path.Combine(uploadsFolder, Path.GetFileName(artwork.FileName));

            // Save file asynchronously
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await artwork.CopyToAsync(stream);
            }
        }

        TempData["Message"] = "Your application has been submitted!";
        return RedirectToAction("Join");
    }




}