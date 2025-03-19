// RangShala.Models/CartItem.cs
namespace RangShala.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string? Name { get; set; } // Changed to nullable string
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int UserId { get; set; }
        public string? ImagePath { get; set; } // Changed to nullable
        public string? ArtistName { get; set; } // Changed to nullable
        public string? Size { get; set; } // Changed to nullable
        public ApplicationUser User { get; set; }
    }
}