using CryptoManager.Application.DTOs;

namespace CryptoManager.Application.Services.Interfaces
{
    public interface IBinanceService
    {
        string GetUrl(string option);

        BalanceDTO GetTotalBalance(CryptoRateDTO rate);
    }
}
