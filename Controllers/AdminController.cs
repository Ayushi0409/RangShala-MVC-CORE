using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangShala.Data;
using RangShala.Models;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace RangShala.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AdminController(AdminDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            var model = new DashboardViewModel
            {
                Artworks = _context.Artworks.Count(),
                Customers = _context.Customers.Count(),
                Categories = _context.Artworks.Select(a => a.Category).Distinct().Count(),
                Orders = _context.Orders.Count()
            };

            return View("Dashboard", model);
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("AdminEmail") != null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.Email == email && a.Password == password);

            if (admin != null)
            {
                HttpContext.Session.SetString("AdminEmail", admin.Email ?? string.Empty);
                return RedirectToAction("Index");
            }

            ViewBag.Message = "Invalid Email or Password";
            return View();
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            var model = new DashboardViewModel
            {
                Artworks = _context.Artworks.Count(),
                Customers = _context.Customers.Count(),
                Categories = _context.Artworks.Select(a => a.Category).Distinct().Count(),
                Orders = _context.Orders.Count()
            };

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult AddAcrylicPainting()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public IActionResult AddAcrylicPainting(Artwork artwork, IFormFile paintingImage)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                if (paintingImage != null && paintingImage.Length > 0)
                {
                    string imagesFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Images");
                    if (!Directory.Exists(imagesFolder))
                    {
                        Directory.CreateDirectory(imagesFolder);
                    }
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(paintingImage.FileName);
                    string filePath = Path.Combine(imagesFolder, uniqueFileName);
                    try
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            paintingImage.CopyTo(fileStream);
                        }
                        artwork.PaintingImage = "/Images/" + uniqueFileName;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error uploading image: " + ex.Message);
                        return View(artwork);
                    }
                }
                artwork.Category = "Acrylic Painting";
                _context.Artworks.Add(artwork);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(artwork);
        }

        public IActionResult AddOilPainting()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }
            return View("AddAcrylicPainting");
        }

        [HttpPost]
        public IActionResult AddOilPainting(Artwork artwork, IFormFile paintingImage)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                if (paintingImage != null && paintingImage.Length > 0)
                {
                    string imagesFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Images");
                    if (!Directory.Exists(imagesFolder))
                    {
                        Directory.CreateDirectory(imagesFolder);
                    }
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(paintingImage.FileName);
                    string filePath = Path.Combine(imagesFolder, uniqueFileName);
                    try
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            paintingImage.CopyTo(fileStream);
                        }
                        artwork.PaintingImage = "/Images/" + uniqueFileName;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error uploading image: " + ex.Message);
                        return View("AddAcrylicPainting", artwork);
                    }
                }
                artwork.Category = "Oil Painting";
                _context.Artworks.Add(artwork);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("AddAcrylicPainting", artwork);
        }

        public IActionResult AddMandalaArt()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }
            return View("AddAcrylicPainting");
        }

        [HttpPost]
        public IActionResult AddMandalaArt(Artwork artwork, IFormFile paintingImage)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                if (paintingImage != null && paintingImage.Length > 0)
                {
                    string imagesFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Images");
                    if (!Directory.Exists(imagesFolder))
                    {
                        Directory.CreateDirectory(imagesFolder);
                    }
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(paintingImage.FileName);
                    string filePath = Path.Combine(imagesFolder, uniqueFileName);
                    try
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            paintingImage.CopyTo(fileStream);
                        }
                        artwork.PaintingImage = "/Images/" + uniqueFileName;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error uploading image: " + ex.Message);
                        return View("AddAcrylicPainting", artwork);
                    }
                }
                artwork.Category = "Mandala Art";
                _context.Artworks.Add(artwork);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("AddAcrylicPainting", artwork);
        }

        public IActionResult AddAnimeDrawings()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }
            return View("AddAcrylicPainting");
        }

        [HttpPost]
        public IActionResult AddAnimeDrawings(Artwork artwork, IFormFile paintingImage)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                if (paintingImage != null && paintingImage.Length > 0)
                {
                    string imagesFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Images");
                    if (!Directory.Exists(imagesFolder))
                    {
                        Directory.CreateDirectory(imagesFolder);
                    }
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(paintingImage.FileName);
                    string filePath = Path.Combine(imagesFolder, uniqueFileName);
                    try
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            paintingImage.CopyTo(fileStream);
                        }
                        artwork.PaintingImage = "/Images/" + uniqueFileName;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error uploading image: " + ex.Message);
                        return View("AddAcrylicPainting", artwork);
                    }
                }
                artwork.Category = "Anime Drawings";
                _context.Artworks.Add(artwork);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("AddAcrylicPainting", artwork);
        }

        public IActionResult AddDrawing()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }
            return View("AddAcrylicPainting");
        }

        [HttpPost]
        public IActionResult AddDrawing(Artwork artwork, IFormFile paintingImage)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                if (paintingImage != null && paintingImage.Length > 0)
                {
                    string imagesFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Images");
                    if (!Directory.Exists(imagesFolder))
                    {
                        Directory.CreateDirectory(imagesFolder);
                    }
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(paintingImage.FileName);
                    string filePath = Path.Combine(imagesFolder, uniqueFileName);
                    try
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            paintingImage.CopyTo(fileStream);
                        }
                        artwork.PaintingImage = "/Images/" + uniqueFileName;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error uploading image: " + ex.Message);
                        return View("AddAcrylicPainting", artwork);
                    }
                }
                artwork.Category = "Drawing";
                _context.Artworks.Add(artwork);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("AddAcrylicPainting", artwork);
        }
    }
}