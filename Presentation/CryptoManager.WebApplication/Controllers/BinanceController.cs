using CryptoManager.Application.DTOs;
using CryptoManager.Infrastructure.Services.Binance.Interfaces;
using CryptoManager.WebApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace CryptoManager.WebApplication.Controllers
{
    [Route("[controller]")]
    public class BinanceController : Controller
    {
        private readonly IBinanceApiService _service;

        public BinanceController(IBinanceApiService service) => _service = service;

        [HttpPost("PortfolioCalculation")]
        public async Task<IActionResult> PortfolioCalculation()
        {
            BalanceDTO dto = await _service.GetRate();

            BalanceModel model = new BalanceModel()
            {
                DASH = Math.Round(dto.DASH, 6),
                BTC = Math.Round(dto.BTC, 6),
                USDT = Math.Round(dto.USDT, 6),
                XMR = Math.Round(dto.XMR, 6),
                XRP = Math.Round(dto.XRP, 6)
            };

            return Json(new
            {
                data = model
            });
        }
    }
}
