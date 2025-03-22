using CryptoManager.Infrastructure.Responses;

namespace CryptoManager.Infrastructure.Services.Bitfinex.Interfaces
{
    public interface IRestConnector
    {
        Task<IEnumerable<TradeResponse>> GetNewTradesAsync(string pair, int maxCount);
        Task<IEnumerable<CandleResponse>> GetCandleSeriesAsync(string pair, string period, DateTimeOffset? from, DateTimeOffset? to = null, long? count = 0);
        Task<TickerResponce> GetTickerAsync(string pair);
        Task<CurrencyResponse> GetCurrencyAsync();
    }
}
