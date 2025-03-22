using CryptoManager.Application.Common.Constants;
using CryptoManager.Application.Services.Interfaces;
using CryptoManager.Infrastructure.Options;
using CryptoManager.Infrastructure.Responses;
using CryptoManager.Infrastructure.Services.Bitfinex.Interfaces;
using System.Diagnostics.Metrics;
using System.Text;
using System.Text.Json;

namespace CryptoManager.Infrastructure.Services.Bitfinex.Implementations
{
    public class RestConnector : IRestConnector
    {
        private readonly IBitfinexService _bitfinex;

        private readonly IHttpClientFactory _clientFactory;

        public RestConnector(IBitfinexService bitfinex, IHttpClientFactory clientFactory)
        {
            _bitfinex = bitfinex;
            _clientFactory = clientFactory;
        }

        public async Task<IEnumerable<CandleResponse>> GetCandleSeriesAsync(string pair, string period, DateTimeOffset? from, DateTimeOffset? to = null, long? count = 0)
        {
            count = count < 0 ? 0 : count;

            StringBuilder destination = new StringBuilder($"{_bitfinex.GetUrl(BitfinexOption.Url)}/candles/trade:{period}:t{pair}/hist?limit={count}");

            if (from is not null)
                destination.Append($"&start={from.Value.ToUnixTimeMilliseconds()}");
            if (to is not null)
                destination.Append($"&end={to.Value.ToUnixTimeMilliseconds()}");

            using HttpClient client = _clientFactory.CreateClient();
            
            HttpResponseMessage response = await client.GetAsync(destination.ToString());

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException($"Error: {errorMessage}");
            }
            string content = await response.Content.ReadAsStringAsync();

            IEnumerable<JsonElement[]> jsonArray = JsonSerializer.Deserialize<IEnumerable<JsonElement[]>>(content);

            List<CandleResponse> candles = new List<CandleResponse>();

            foreach (JsonElement[] json in jsonArray)
            {
                CandleResponse candle = new CandleResponse()
                {
                    Pair = pair,
                    OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(json[0].GetInt64()),
                    OpenPrice = json[1].GetDecimal(),
                    ClosePrice = json[2].GetDecimal(),
                    HighPrice = json[3].GetDecimal(),
                    LowPrice = json[4].GetDecimal(), 
                    TotalPrice = (json[3].GetDecimal() + json[4].GetDecimal()) / 2 * json[5].GetDecimal(),
                    TotalVolume = json[5].GetDecimal()
                };
                candles.Add(candle);
            }
            return candles;
        }

        public async Task<CurrencyResponse> GetCurrencyAsync()
        {
            string destination = $"{_bitfinex.GetUrl(BitfinexOption.Url)}/conf/pub:list:pair:margin";

            using HttpClient client = _clientFactory.CreateClient();

            HttpResponseMessage response = await client.GetAsync(destination);

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException($"Error: {errorMessage}");
            }
            string content = await response.Content.ReadAsStringAsync();

            JsonElement[] json = JsonSerializer.Deserialize<JsonElement[]>(content);

            List<string> pairs = new List<string>();
            foreach (JsonElement element in json[0].EnumerateArray())
            {
                pairs.Add(element.GetString());
            }

            CurrencyResponse currency = new CurrencyResponse()
            {
                Pairs = pairs
            };

            return currency;
        }

        public async Task<IEnumerable<TradeResponse>> GetNewTradesAsync(string pair, int maxCount)
        {
            maxCount = maxCount < 0 ? 0 : maxCount;

            string destination = $"{_bitfinex.GetUrl(BitfinexOption.Url)}/trades/t{pair}/hist?limit={maxCount}";

            using HttpClient client = _clientFactory.CreateClient();
            
            HttpResponseMessage response = await client.GetAsync(destination);

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException($"Error: {errorMessage}");
            }
            string content = await response.Content.ReadAsStringAsync();

            IEnumerable<JsonElement[]> jsonArray = JsonSerializer.Deserialize<IEnumerable<JsonElement[]>>(content);

            List<TradeResponse> trades = new List<TradeResponse>();

            foreach (JsonElement[] json in jsonArray)
            {
                TradeResponse trade = new TradeResponse()
                {
                    Id = json[0].GetInt64().ToString(),
                    Time = DateTimeOffset.FromUnixTimeMilliseconds(json[1].GetInt64()),
                    Amount = json[2].GetDecimal(),
                    Price = json[3].GetDecimal(),
                    Pair = pair,
                    Side = json[2].GetDecimal() > 0 ? TransactionParty.Buy : TransactionParty.Sell
                };
                trades.Add(trade);
            }
            return trades;    
        }
        public async Task<TickerResponce> GetTickerAsync(string pair)
        {
            string destination = $"{_bitfinex.GetUrl(BitfinexOption.Url)}/ticker/t{pair}";

            using HttpClient client = _clientFactory.CreateClient();
            
            HttpResponseMessage response = await client.GetAsync(destination);

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException($"Error: {errorMessage}");
            }
            string content = await response.Content.ReadAsStringAsync();

            JsonElement[] json = JsonSerializer.Deserialize<JsonElement[]>(content);

            TickerResponce ticker = new TickerResponce()
            {
                Pair = pair,
                BidPrice = json[0].GetDecimal(),
                BidSize = json[1].GetDecimal(),
                AskPrice = json[2].GetDecimal(),
                AskSize = json[3].GetDecimal(),
                DailyChange = json[4].GetDecimal(),
                DailyChangeRelative = json[5].GetDecimal(),
                LastPrice = json[6].GetDecimal(),
                Volume = json[7].GetDecimal(),
                High = json[8].GetDecimal(),
                Low = json[9].GetDecimal(),
            };
            return ticker;       
        }
    }
}
