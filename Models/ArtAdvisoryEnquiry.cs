namespace RangShala.Models
{
    public class ArtAdvisoryEnquiry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string MobileNo { get; set; }
        public string Enquiry { get; set; }
        public DateTime SubmissionDate { get; set; }
    }
}