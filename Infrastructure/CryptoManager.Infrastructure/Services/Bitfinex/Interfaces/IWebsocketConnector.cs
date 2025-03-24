using CryptoManager.Infrastructure.Responses.Bitfinex;
using System.Diagnostics;

namespace CryptoManager.Infrastructure.Services.Bitfinex.Interfaces
{
    public interface IWebsocketConnector
    {
        Task ConnectAsync();
        event Action<TradeResponse> NewBuyTrade;
        event Action<TradeResponse> NewSellTrade;
        void SubscribeTrades(string pair, int maxCount = 100);
        void UnsubscribeTrades(string pair);

        event Action<CandleResponse> CandleSeriesProcessing;
        void SubscribeCandles(string pair, string period, DateTimeOffset? from = null, DateTimeOffset? to = null, long? count = 0);
        void UnsubscribeCandles(string pair);
    }
}
