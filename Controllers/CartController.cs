
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
                    // Drawings page items
                    case "Shree Nathji":
                        newItem.ImagePath = "~/Images/shreenathjii.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Tiger":
                        newItem.ImagePath = "~/Images/tiger.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Ganpati Bappa":
                        newItem.ImagePath = "~/Images/bappa.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Wolf":
                        newItem.ImagePath = "~/Images/cat.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Buddha":
                        newItem.ImagePath = "~/Images/buddha.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Eye":
                        newItem.ImagePath = "~/Images/eye.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Ostrich":
                        newItem.ImagePath = "~/Images/bird.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Deer":
                        newItem.ImagePath = "~/Images/deer.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "INK":
                        newItem.ImagePath = "~/Images/ink.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Nature":
                        newItem.ImagePath = "~/Images/naturee.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Ganpati":
                        newItem.ImagePath = "~/Images/Ganpati.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Cute Girl":
                        newItem.ImagePath = "~/Images/girl.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Panihari":
                        newItem.ImagePath = "~/Images/Panihari.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;

                    // Oil Painting page items
                    case "Ganpati Bappa ":
                        newItem.ImagePath = "~/Images/oil1.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Window Painting":
                        newItem.ImagePath = "~/Images/oil2.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Lady":
                        newItem.ImagePath = "~/Images/oil3.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Lazy Girl":
                        newItem.ImagePath = "~/Images/oil4.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Fruits":
                        newItem.ImagePath = "~/Images/oil6.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Rose":
                        newItem.ImagePath = "~/Images/oil7.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Love Birds":
                        newItem.ImagePath = "~/Images/oil8.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Bird":
                        newItem.ImagePath = "~/Images/oil9.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "House":
                        newItem.ImagePath = "~/Images/oil10.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;

                    // Mandala Art page items
                    case "Mandala Bhat":
                        newItem.ImagePath = "~/Images/mandala1.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Wall Decor":
                        newItem.ImagePath = "~/Images/mandala2.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Shiv":
                        newItem.ImagePath = "~/Images/mandala3.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "eye":
                        newItem.ImagePath = "~/Images/mandala4.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Beautiful Design":
                        newItem.ImagePath = "~/Images/mandala5.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "TajMahal":
                        newItem.ImagePath = "~/Images/mandala6.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Owl":
                        newItem.ImagePath = "~/Images/mandala8.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Hanuman Dada":
                        newItem.ImagePath = "~/Images/mandala9.jpg";
                        newItem.ArtistName = "Ayushi Babariya";
                        newItem.Size = "24in x 24in";
                        break;

                    // Anime page items
                    case "Giyu Tomiyoka":
                        newItem.ImagePath = "~/Images/anime1.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Muichiro Tokito":
                        newItem.ImagePath = "~/Images/anime2.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Ayanokoji":
                        newItem.ImagePath = "~/Images/anime3.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Mikey Sano":
                        newItem.ImagePath = "~/Images/anime4.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Minato Namikaze":
                        newItem.ImagePath = "~/Images/anime5.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Itachi Uchiha":
                        newItem.ImagePath = "~/Images/anime6.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Miku":
                        newItem.ImagePath = "~/Images/anime7.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;
                    case "Killua":
                        newItem.ImagePath = "~/Images/anime8.jpg";
                        newItem.ArtistName = "Niyati Agravat";
                        newItem.Size = "24in x 24in";
                        break;

                    // Default case
                    default:
                        newItem.ImagePath = "~/Images/default.jpg";
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

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var user = HttpContext.Session.GetObjectFromJson<ApplicationUser>("User");
            if (user == null)
            {
                return Json(new { success = false, message = "Please log in to delete items from the cart." });
            }

            var cartItem = _context.CartItems
                .FirstOrDefault(c => c.Id == id && c.UserId == user.Id);
            if (cartItem == null)
            {
                return Json(new { success = false, message = "Item not found in your cart." });
            }

            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();

            return Json(new { success = true, message = "Item removed from cart." });
        }
    }

    public class CartItemDTO
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}