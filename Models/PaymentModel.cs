namespace RangShala.Models
{
    public class PaymentModel
    {
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public string SecurityCode { get; set; }
    }
}