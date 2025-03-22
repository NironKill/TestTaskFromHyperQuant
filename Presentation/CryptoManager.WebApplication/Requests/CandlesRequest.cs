namespace CryptoManager.WebApplication.Requests
{
    public class CandlesRequest
    {
        public string Pair { get; set; }
        public string Period { get; set; }
        public DateTimeOffset? From { get; set; } = null;
        public DateTimeOffset? To { get; set; } = null;
        public long Count { get; set; } = 0;
    }
}
