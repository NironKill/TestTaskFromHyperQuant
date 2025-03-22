using CryptoManager.Application.Services.Interfaces;

namespace CryptoManager.Application.Services.Implementations
{
    public class BitfinexService : IBitfinexService
    {
        private readonly IApiService _api;

        public BitfinexService(IApiService api) => _api = api;

        public string GetUrl(string option) => _api.GetApiConfiguration(option);
    }
}
