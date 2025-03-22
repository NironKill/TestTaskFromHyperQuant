using CryptoManager.Infrastructure.Responses;
using CryptoManager.Infrastructure.Services.Bitfinex.Interfaces;

namespace CryptoManager.Infrastructure.Services.Bitfinex.Implementations
{
    public class WebsocketConnector : IWebsocketConnector
    {
        public event Action<TradeResponse> NewBuyTrade;
        public event Action<TradeResponse> NewSellTrade;
        public event Action<CandleResponse> CandleSeriesProcessing;

        public void SubscribeCandles(string pair, int periodInSec, DateTimeOffset? from = null, DateTimeOffset? to = null, long? count = 0)
        {
            throw new NotImplementedException();
        }
        public void SubscribeTrades(string pair, int maxCount = 100)
        {
            throw new NotImplementedException();
        }
        public void UnsubscribeCandles(string pair)
        {
            throw new NotImplementedException();
        }
        public void UnsubscribeTrades(string pair)
        {
            throw new NotImplementedException();
        }
    }
}
