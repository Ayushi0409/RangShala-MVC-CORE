using System.ComponentModel.DataAnnotations;

namespace RangShala.Models
{
    public class Artwork
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Painting Name is required")]
        public string? PaintingName { get; set; }

        [Required(ErrorMessage = "Painting Image is required")]
        public string? PaintingImage { get; set; }

        [Required(ErrorMessage = "Painting Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number")]
        public decimal PaintingPrice { get; set; }

        [Required(ErrorMessage = "Artist Name is required")]
        public string? ArtistName { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string? Category { get; set; }
    }
}