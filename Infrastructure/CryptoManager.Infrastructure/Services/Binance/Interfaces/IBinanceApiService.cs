using CryptoManager.Application.DTOs;

namespace CryptoManager.Infrastructure.Services.Binance.Interfaces
{
    public interface IBinanceApiService
    {
        Task<BalanceDTO> GetRate();
    }
}
