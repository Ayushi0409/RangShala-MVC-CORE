// Models/OtpRecord.cs
namespace RangShala.Models
{
    public class OtpRecord
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Otp { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsUsed { get; set; }
    }
}