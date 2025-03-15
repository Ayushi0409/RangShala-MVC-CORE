using Microsoft.AspNetCore.Mvc;
using RangShala.Data;
using RangShala.Helpers;
using RangShala.Models;

namespace RangShala.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var user = HttpContext.Session.GetObjectFromJson<ApplicationUser>("User");
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = _context.CartItems
                .Where(c => c.UserId == user.Id)
                .ToList();
            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart([FromBody] CartItemDTO itemDTO)
        {
            if (itemDTO == null || string.IsNullOrEmpty(itemDTO.Name) || itemDTO.Price <= 0)
            {
                return Json(new { success = false, message = "Invalid item data." });
            }

            var user = HttpContext.Session.GetObjectFromJson<ApplicationUser>("User");
            if (user == null)
            {
                return Json(new { success = false, message = "Please log in to add items to cart." });
            }

            var existingItem = _context.CartItems
                .FirstOrDefault(i => i.Name == itemDTO.Name && i.UserId == user.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += 1;
                _context.SaveChanges();
            }
            else
            {
                var newItem = new CartItem
                {
                    Name = itemDTO.Name,
                    Quantity = 1,
                    Price = itemDTO.Price,
                    UserId = user.Id
                };

                switch (itemDTO.Name)
                {
                    case "Shree Nathji":
                        newItem.ImagePath = "/Images/Shreenathji.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Gouache Painting":
                        newItem.ImagePath = "/Images/gouachepainting.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Ganpati Bappa":
                        newItem.ImagePath = "/Images/Ganpatibappa.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Night Pai":
                        newItem.ImagePath = "/Images/NightPai.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "House Painting":
                        newItem.ImagePath = "/Images/HousePainting.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Rainy Day":
                        newItem.ImagePath = "/Images/RainyDay.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Hill View":
                        newItem.ImagePath = "/Images/HillView.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Hanuman Dada":
                        newItem.ImagePath = "/Images/HanumanDada.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Sunrise Painting":
                        newItem.ImagePath = "/Images/SunrisePainting.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    default:
                        newItem.ImagePath = "/Images/default.jpg";
                        newItem.ArtistName = "Unknown Artist";
                        newItem.Size = "24in x 24in";
                        break;
                }

                _context.CartItems.Add(newItem);
                _context.SaveChanges();
            }

            return Json(new { success = true, message = $"{itemDTO.Name} added to cart!" });
        }

        [HttpGet]
        public IActionResult IsUserLoggedIn()
        {
            var user = HttpContext.Session.GetObjectFromJson<ApplicationUser>("User");
            return Json(user != null);
        }
    }

    public class CartItemDTO
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}