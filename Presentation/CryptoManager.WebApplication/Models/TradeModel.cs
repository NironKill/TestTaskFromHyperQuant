namespace CryptoManager.WebApplication.Models
{
    public class TradeModel
    {
        public string Pair { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public string Side { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Id { get; set; }
    }
}
