namespace CryptoManager.Infrastructure.Responses.Bitfinex
{
    public class TradeResponse
    {
        /// <summary>
        /// Currency pair
        /// </summary>
        public string Pair { get; set; }

        /// <summary>
        /// Trade price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Trade volume
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Direction (buy/sell)
        /// </summary>
        public string Side { get; set; }

        /// <summary>
        /// Trading time
        /// </summary>
        public DateTimeOffset Time { get; set; }


        /// <summary>
        /// Id trade
        /// </summary>
        public string Id { get; set; }
    }
}
