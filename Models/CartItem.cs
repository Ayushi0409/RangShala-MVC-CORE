namespace RangShala.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public string ArtistName { get; set; }
        public string Size { get; set; }
        public int? UserId { get; set; } // Add this to link to ApplicationUser
        public ApplicationUser User { get; set; } // Navigation property
    }
}