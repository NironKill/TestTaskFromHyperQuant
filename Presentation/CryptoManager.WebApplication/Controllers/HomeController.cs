using CryptoManager.Infrastructure.Hubs;
using CryptoManager.Infrastructure.Responses.Bitfinex;
using CryptoManager.Infrastructure.Services.Bitfinex.Interfaces;
using CryptoManager.WebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace CryptoManager.WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRestConnector _restConnector;
        private readonly IWebsocketConnector _websocketConnector;
        private readonly IHubContext<TradeHub> _hubContext;

        public HomeController(IRestConnector restConnector, IWebsocketConnector websocketConnector, IHubContext<TradeHub> hubContext)
        {
            _restConnector = restConnector;
            _websocketConnector = websocketConnector;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            await _websocketConnector.ConnectAsync();
            _websocketConnector.NewBuyTrade += async trade =>
            {
                await _hubContext.Clients.All.SendAsync("ReceiveTrade", trade.Id, trade.Pair, trade.Side, trade.Price, trade.Amount, trade.Time);
            };

            _websocketConnector.NewSellTrade += async trade =>
            {
                await _hubContext.Clients.All.SendAsync("ReceiveTrade", trade.Id, trade.Pair, trade.Side, trade.Price, trade.Amount, trade.Time);
            };

            _websocketConnector.CandleSeriesProcessing += async candle =>
            {
                await _hubContext.Clients.All.SendAsync("ReceiveCandle",
                    candle.Pair,
                    candle.OpenPrice,
                    candle.ClosePrice,
                    candle.HighPrice,
                    candle.LowPrice,
                    candle.TotalVolume,
                    candle.TotalPrice,
                    candle.OpenTime.ToUnixTimeMilliseconds());
            };

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrencyPairs()
        {
            CurrencyResponse currency = await _restConnector.GetCurrencyAsync();

            CurrencyModel model = new CurrencyModel()
            {
                Pairs = currency.Pairs,
            };
            return Json(new { pairs = model.Pairs });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
