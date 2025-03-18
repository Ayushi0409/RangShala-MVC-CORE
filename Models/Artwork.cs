namespace RangShala.Models
{
    public class Artwork
    {
        public int Id { get; set; }
        public required string PaintingName { get; set; } // Use 'required' for C# 11.0+
        public required string PaintingImage { get; set; }
        public decimal PaintingPrice { get; set; }
        public required string ArtistName { get; set; }
        public required string Category { get; set; }
    }
}