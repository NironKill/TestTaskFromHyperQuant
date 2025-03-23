using System.Text.Json.Serialization;

namespace CryptoManager.Infrastructure.Responses.Binance
{
    public class CryptoRateResponse
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; init; }
        [JsonPropertyName("price")]
        public string Price { get; init; }
    }
}
