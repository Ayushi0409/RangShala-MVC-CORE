namespace RangShala.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string EmailVerificationToken { get; set; }
        public bool IsEmailVerified { get; set; }
    }
}
