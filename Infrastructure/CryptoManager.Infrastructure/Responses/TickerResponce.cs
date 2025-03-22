namespace CryptoManager.Infrastructure.Responses
{
    public class TickerResponce
    {
        /// <summary>
        /// Currency pair
        /// </summary>
        public string Pair { get; set; }

        /// <summary>
        /// Price of last highest bid
        /// </summary>
        public decimal BidPrice { get; set; }

        /// <summary>
        /// Sum of the 25 highest bid sizes
        /// </summary>
        public decimal BidSize { get; set; }

        /// <summary>
        /// Price of last lowest ask
        /// </summary>
        public decimal AskPrice { get; set; }

        /// <summary>
        /// Sum of the 25 lowest ask sizes
        /// </summary>
        public decimal AskSize { get; set; }

        /// <summary>
        /// Amount that the last price has changed since yesterday
        /// </summary>
        public decimal DailyChange { get; set; }

        /// <summary>
        /// Relative price change since yesterday (percentage)
        /// </summary>
        public decimal DailyChangeRelative { get; set; }

        /// <summary>
        /// Price of the last trade
        /// </summary>
        public decimal LastPrice { get; set; }

        /// <summary>
        /// Daily volume
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// Daily high
        /// </summary>
        public decimal High { get; set; }

        /// <summary>
        /// Daily low
        /// </summary>
        public decimal Low { get; set; }
    }
}
