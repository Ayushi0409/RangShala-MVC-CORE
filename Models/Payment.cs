namespace RangShala.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? RazorpayOrderId { get; set; }    // Nullable
        public string? RazorpayPaymentId { get; set; }  // Nullable
        public string? RazorpaySignature { get; set; }  // Nullable
        public decimal Amount { get; set; }
        public string? Currency { get; set; }           // Nullable
        public string? Email { get; set; }              // Nullable
        public string? Name { get; set; }               // Nullable
        public string? Contact { get; set; }            // Nullable
        public DateTime PaymentDate { get; set; }
        public bool IsSuccessful { get; set; }

        public ApplicationUser? User { get; set; }      // Nullable navigation property
    }
}