using CryptoManager.Application.DTOs;
using CryptoManager.Application.Services.Interfaces;
using CryptoManager.Domain;

namespace CryptoManager.Application.Services.Implementations
{
    public class BinanceService : IBinanceService
    {
        private readonly IApiService _api;
        private readonly Portfolio _portfolio;

        public BinanceService(IApiService api, Portfolio portfolio)
        {
            _api = api;
            _portfolio = portfolio;
        }

        public BalanceDTO GetTotalBalance(CryptoRateDTO rate)
        {
            decimal totalUSDT = _portfolio.BTC * rate.BTCUSDT +
                _portfolio.DASH * rate.DASHUSDT +
                _portfolio.XMR * rate.XMRUSDT +
                _portfolio.XRP * rate.XRPUSDT;

            BalanceDTO balance = new BalanceDTO()
            {
                USDT = totalUSDT,
                BTC = totalUSDT / rate.BTCUSDT,
                DASH = totalUSDT / rate.DASHUSDT,
                XRP = totalUSDT / rate.XRPUSDT,
                XMR = totalUSDT / rate.XMRUSDT,
            };

            return balance;
        }

        public string GetUrl(string option) => _api.GetApiConfiguration(option);
    }
}
