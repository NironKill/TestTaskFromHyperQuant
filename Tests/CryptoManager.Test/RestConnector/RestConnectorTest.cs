using CryptoManager.Application.Common.Constants;
using CryptoManager.Application.Services.Interfaces;
using CryptoManager.Infrastructure.Options;
using CryptoManager.Infrastructure.Responses.Bitfinex;
using CryptoManager.Infrastructure.Services.Bitfinex.Implementations;
using Moq;
using System.Net;
using System.Text.Json;

namespace CryptoManager.Test;

public class RestConnectorTests : HttpClientFactory
{
    private readonly Mock<IBitfinexService> _mockBitfinexService;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly RestConnector _restConnector;

    public RestConnectorTests()
    {
        _mockBitfinexService = new Mock<IBitfinexService>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _restConnector = new RestConnector(_mockBitfinexService.Object, _mockHttpClientFactory.Object);
    }

    [Fact]
    public async Task GetNewTradesAsync_ReturnsTrades()
    {
        // Arrange
        string pair = "BTCUSD";
        int maxCount = 5;

        string jsonResponse = JsonSerializer.Serialize(new List<JsonElement[]>
        {
            new JsonElement[]
            {
                JsonDocument.Parse("12345").RootElement,
                JsonDocument.Parse("1633024800000").RootElement,
                JsonDocument.Parse("1.5").RootElement,
                JsonDocument.Parse("50000").RootElement
            }
        });

        HttpClient httpClient = CreateHttpClient(HttpStatusCode.OK, jsonResponse);
        _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
        _mockBitfinexService.Setup(x => x.GetUrl(BitfinexOption.Url)).Returns("https://api.bitfinex.com/v2");

        // Act
        IEnumerable<TradeResponse> result = await _restConnector.GetNewTradesAsync(pair, maxCount);

        // Assert
        Assert.Single(result);
        TradeResponse trade = result.First();
        Assert.Equal("12345", trade.Id);
        Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1633024800000), trade.Time);
        Assert.Equal(1.5m, trade.Amount);
        Assert.Equal(50000m, trade.Price);
        Assert.Equal(pair, trade.Pair);
        Assert.Equal(TransactionParty.Buy, trade.Side);
    }

    [Fact]
    public async Task GetCandleSeriesAsync_ReturnsCandles()
    {
        // Arrange
        string pair = "BTCUSD";
        string period = "1m";
        DateTimeOffset from = DateTimeOffset.UtcNow.AddHours(-1);
        DateTimeOffset to = DateTimeOffset.UtcNow;
        int count = 10;

        string jsonResponse = JsonSerializer.Serialize(new List<JsonElement[]>
        {
            new JsonElement[]
            {
                JsonDocument.Parse("1633024800000").RootElement,
                JsonDocument.Parse("50000").RootElement,
                JsonDocument.Parse("51000").RootElement,
                JsonDocument.Parse("52000").RootElement,
                JsonDocument.Parse("49000").RootElement,
                JsonDocument.Parse("100").RootElement
            }
        });

        HttpClient httpClient = CreateHttpClient(HttpStatusCode.OK, jsonResponse);
        _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
        _mockBitfinexService.Setup(x => x.GetUrl(BitfinexOption.Url)).Returns("https://api.bitfinex.com/v2");

        // Act
        IEnumerable<CandleResponse> result = await _restConnector.GetCandleSeriesAsync(pair, period, from, to, count);

        // Assert
        Assert.Single(result);
        CandleResponse candle = result.First();
        Assert.Equal(pair, candle.Pair);
        Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1633024800000), candle.OpenTime);
        Assert.Equal(50000,  candle.OpenPrice);
        Assert.Equal(51000, candle.ClosePrice);
        Assert.Equal(52000, candle.HighPrice);
        Assert.Equal(49000, candle.LowPrice);
        Assert.Equal(50500 * 100, candle.TotalPrice); 
        Assert.Equal(100, candle.TotalVolume);
    }    
}
