using CryptoManager.Application.Services.Implementations;
using CryptoManager.Application.Services.Interfaces;
using CryptoManager.Infrastructure.Services.Bitfinex.Implementations;
using CryptoManager.Infrastructure.Services.Bitfinex.Interfaces;

namespace CryptoManager.WebApplication.Configurations
{
    internal static class ServiceExtention
    {
        internal static WebApplicationBuilder ConfigureService(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IApiService, ApiService>();
            builder.Services.AddScoped<IBitfinexService, BitfinexService>();
            builder.Services.AddScoped<IRestConnector, RestConnector>();
            builder.Services.AddScoped<IWebsocketConnector, WebsocketConnector>();

            return builder;
        }
    }
}
