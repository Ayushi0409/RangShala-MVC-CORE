using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

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

}