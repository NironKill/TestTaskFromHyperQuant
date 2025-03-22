namespace CryptoManager.Infrastructure.Responses
{
    public class CandleResponse
    {
        /// <summary>
        /// Currency pair
        /// </summary>
        public string Pair { get; set; }

        /// <summary>
        /// Opening price
        /// </summary>
        public decimal OpenPrice { get; set; }

        /// <summary>
        /// Maximum price
        /// </summary>
        public decimal HighPrice { get; set; }

        /// <summary>
        /// Minimum price
        /// </summary>
        public decimal LowPrice { get; set; }

        /// <summary>
        /// Closing price
        /// </summary>
        public decimal ClosePrice { get; set; }


        /// <summary>
        /// Partial (Total amount of transactions)
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Partial (Total volume)
        /// </summary>
        public decimal TotalVolume { get; set; }

        /// <summary>
        /// Time
        /// </summary>
        public DateTimeOffset OpenTime { get; set; }
    }
}
