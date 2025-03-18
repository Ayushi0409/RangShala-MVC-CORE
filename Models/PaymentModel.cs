namespace RangShala.Models
{
    public class PaymentModel
    {
        public string? OrderId { get; set; }        // Nullable to avoid constructor warning
        public decimal Amount { get; set; }         // Value type, no null issue
        public string? Currency { get; set; }       // Nullable
        public string? KeyId { get; set; }          // Nullable
        public string? Email { get; set; }          // Nullable
        public string? Name { get; set; }           // Nullable
        public string? Contact { get; set; }        // Nullable
    }
}