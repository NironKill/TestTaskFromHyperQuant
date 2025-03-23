using CryptoManager.Application.Services.Implementations;
using CryptoManager.Application.Services.Interfaces;
using CryptoManager.Domain;
using CryptoManager.Infrastructure.Services.Binance.Implementations;
using CryptoManager.Infrastructure.Services.Binance.Interfaces;
using CryptoManager.Infrastructure.Services.Bitfinex.Implementations;
using CryptoManager.Infrastructure.Services.Bitfinex.Interfaces;

namespace CryptoManager.WebApplication.Configurations
{
    internal static class ServiceExtention
    {
        internal static WebApplicationBuilder ConfigureService(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IRestConnector, RestConnector>();
            builder.Services.AddSingleton<IBinanceService, BinanceService>();
            builder.Services.AddScoped<IBinanceApiService, BinanceApiService>();

            builder.Services.AddSingleton<IApiService, ApiService>();
            builder.Services.AddSingleton<IBitfinexService, BitfinexService>();
            builder.Services.AddSingleton<IWebsocketConnector, WebsocketConnector>();
            builder.Services.AddSingleton<Portfolio>();

            return builder;
        }
    }
}
