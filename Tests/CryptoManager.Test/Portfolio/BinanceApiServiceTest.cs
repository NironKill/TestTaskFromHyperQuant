using CryptoManager.Application.DTOs;
using CryptoManager.Application.Services.Interfaces;
using CryptoManager.Infrastructure.Options;
using CryptoManager.Infrastructure.Responses.Binance;
using CryptoManager.Infrastructure.Services.Binance.Implementations;
using Moq;
using System.Net;
using System.Text.Json;

namespace CryptoManager.Test;

public class BinanceApiServiceTest : HttpClientFactory
{
    private readonly Mock<IBinanceService> _mockBinanceService;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly BinanceApiService _binanceApiService;

    public BinanceApiServiceTest()
    {
        _mockBinanceService = new Mock<IBinanceService>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _binanceApiService = new BinanceApiService(_mockBinanceService.Object, _mockHttpClientFactory.Object);
    }

    [Fact]
    public async Task GetRate_ReturnsBalanceDTO()
    {
        // Arrange
        string jsonResponse = JsonSerializer.Serialize(new List<CryptoRateResponse>
        {
            new CryptoRateResponse { Symbol = "XRPUSDT", Price = "0.50" },
            new CryptoRateResponse { Symbol = "DASHUSDT", Price = "100.00" },
            new CryptoRateResponse { Symbol = "BTCUSDT", Price = "50000.00" },
            new CryptoRateResponse { Symbol = "XMRUSDT", Price = "200.00" }
        });

        HttpClient httpClient = CreateHttpClient(HttpStatusCode.OK, jsonResponse);
        _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
        _mockBinanceService.Setup(x => x.GetUrl(BinanceOption.Url)).Returns("https://api.binance.com");

        BalanceDTO expectedBalanceDTO = new BalanceDTO
        {
            USDT = 75500.00m, 
            BTC = 1.51m,      
            DASH = 755.00m,    
            XRP = 151000.00m,  
            XMR = 377.50m
        };

        _mockBinanceService.Setup(x => x.GetTotalBalance(It.IsAny<CryptoRateDTO>())).Returns(expectedBalanceDTO);

        // Act
        BalanceDTO result = await _binanceApiService.GetRate();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedBalanceDTO.XRP, result.XRP);
        Assert.Equal(expectedBalanceDTO.DASH, result.DASH);
        Assert.Equal(expectedBalanceDTO.BTC, result.BTC);
        Assert.Equal(expectedBalanceDTO.XMR, result.XMR);
    }
}
