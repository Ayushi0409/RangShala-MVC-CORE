namespace RangShala.Models
{
    public class MapData
    {
        public int Id { get; set; } // Added primary key for EF Core
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Title { get; set; }
    }
}