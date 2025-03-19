using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangShala.Data;
using RangShala.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RangShala.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminDbContext _adminContext;
        private readonly ApplicationDbContext _userContext;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AdminController(AdminDbContext adminContext, ApplicationDbContext userContext, IWebHostEnvironment hostingEnvironment)
        {
            _adminContext = adminContext ?? throw new ArgumentNullException(nameof(adminContext));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            var model = new DashboardViewModel
            {
                Artworks = _adminContext.Artworks.Count(),
                Customers = _adminContext.Customers.Count(),
                Categories = _adminContext.Artworks.Select(a => a.Category).Distinct().Count(),
                Orders = _adminContext.Orders.Count(),
                ArtworkList = _adminContext.Artworks.ToList() // For the popup
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
            var admin = _adminContext.Admins.FirstOrDefault(a => a.Email == email && a.Password == password);

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
                Artworks = _adminContext.Artworks.Count(),
                Customers = _adminContext.Customers.Count(),
                Categories = _adminContext.Artworks.Select(a => a.Category).Distinct().Count(),
                Orders = _adminContext.Orders.Count(),
                ArtworkList = _adminContext.Artworks.ToList()
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
        public async Task<IActionResult> AddAcrylicPainting(Artwork artwork, IFormFile paintingImage)
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
                            await paintingImage.CopyToAsync(fileStream);
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
                _adminContext.Artworks.Add(artwork);
                _userContext.Artworks.Add(artwork); // Add to user DB
                await _adminContext.SaveChangesAsync();
                await _userContext.SaveChangesAsync();
                TempData["Message"] = "Artwork added successfully!";
                // Redirect to the user-facing AcrylicPaintings page instead of Index
                return RedirectToAction("AcrylicPaintings", "Home");
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
        public async Task<IActionResult> AddOilPainting(Artwork artwork, IFormFile paintingImage)
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
                            await paintingImage.CopyToAsync(fileStream);
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
                _adminContext.Artworks.Add(artwork);
                _userContext.Artworks.Add(artwork);
                await _adminContext.SaveChangesAsync();
                await _userContext.SaveChangesAsync();
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
        public async Task<IActionResult> AddMandalaArt(Artwork artwork, IFormFile paintingImage)
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
                            await paintingImage.CopyToAsync(fileStream);
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
                _adminContext.Artworks.Add(artwork);
                _userContext.Artworks.Add(artwork);
                await _adminContext.SaveChangesAsync();
                await _userContext.SaveChangesAsync();
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
        public async Task<IActionResult> AddAnimeDrawings(Artwork artwork, IFormFile paintingImage)
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
                            await paintingImage.CopyToAsync(fileStream);
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
                _adminContext.Artworks.Add(artwork);
                _userContext.Artworks.Add(artwork);
                await _adminContext.SaveChangesAsync();
                await _userContext.SaveChangesAsync();
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
        public async Task<IActionResult> AddDrawing(Artwork artwork, IFormFile paintingImage)
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
                            await paintingImage.CopyToAsync(fileStream);
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
                _adminContext.Artworks.Add(artwork);
                _userContext.Artworks.Add(artwork);
                await _adminContext.SaveChangesAsync();
                await _userContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View("AddAcrylicPainting", artwork);
        }

        public IActionResult ViewAcrylicPaintings()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }
            var acrylicPaintings = _adminContext.Artworks.Where(a => a.Category == "Acrylic Painting").ToList();
            return View(acrylicPaintings);
        }

        public IActionResult ViewOilPaintings()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }
            var oilPaintings = _adminContext.Artworks.Where(a => a.Category == "Oil Painting").ToList();
            return View(oilPaintings);
        }

        public IActionResult ViewMandalaArt()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }
            var mandalaArt = _adminContext.Artworks.Where(a => a.Category == "Mandala Art").ToList();
            return View(mandalaArt);
        }

        public IActionResult ViewAnimeDrawings()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }
            var animeDrawings = _adminContext.Artworks.Where(a => a.Category == "Anime Drawings").ToList();
            return View(animeDrawings);
        }

        public IActionResult ViewDrawings()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }
            var drawings = _adminContext.Artworks.Where(a => a.Category == "Drawing").ToList();
            return View(drawings);
        }

        [HttpPost]
        public IActionResult DeleteArtwork(int id)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            var artwork = _adminContext.Artworks.FirstOrDefault(a => a.Id == id);
            if (artwork != null)
            {
                _adminContext.Artworks.Remove(artwork);
                _userContext.Artworks.Remove(artwork);
                _adminContext.SaveChanges();
                _userContext.SaveChanges();
            }

            return artwork?.Category switch
            {
                "Acrylic Painting" => RedirectToAction("ViewAcrylicPaintings"),
                "Oil Painting" => RedirectToAction("ViewOilPaintings"),
                "Mandala Art" => RedirectToAction("ViewMandalaArt"),
                "Anime Drawings" => RedirectToAction("ViewAnimeDrawings"),
                "Drawing" => RedirectToAction("ViewDrawings"),
                _ => RedirectToAction("Index")
            };
        }
        public IActionResult ViewCustomer()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            var customers = _adminContext.Customers.ToList();
            return View(customers);
        }
        [HttpPost]
        public IActionResult DeleteCustomer(int id)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            var customer = _adminContext.Customers.FirstOrDefault(c => c.Id == id);
            if (customer != null)
            {
                _adminContext.Customers.Remove(customer);
                _adminContext.SaveChanges();
                TempData["SuccessMessage"] = "Customer deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Customer not found.";
            }

            return RedirectToAction("ViewCustomer");
        }
    }
}