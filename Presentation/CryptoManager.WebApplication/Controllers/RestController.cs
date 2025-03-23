using CryptoManager.Infrastructure.Responses.Bitfinex;
using CryptoManager.Infrastructure.Services.Bitfinex.Interfaces;
using CryptoManager.WebApplication.Models;
using CryptoManager.WebApplication.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CryptoManager.WebApplication.Controllers
{
    [Route("[controller]")]
    public class RestController : Controller
    {
        private readonly IRestConnector _restConnector;

        public RestController(IRestConnector restConnector) => _restConnector = restConnector;

        [HttpGet("Manage")]
        public IActionResult Manage()
        {
            return View();
        }

        [HttpPost("TickerTableGeneration")]
        public async Task<IActionResult> TickerTableGeneration([FromBody] TickerRequest request)
        {
            TickerResponce ticker = await _restConnector.GetTickerAsync(request.Pair);

            TickerModel model = new TickerModel()
            {
                Pair = ticker.Pair,
                AskPrice = ticker.AskPrice,
                AskSize = ticker.AskSize,
                BidPrice = ticker.BidPrice,
                BidSize = ticker.BidSize,
                DailyChange = ticker.DailyChange,
                LastPrice = ticker.LastPrice,
                DailyChangeRelative = ticker.DailyChangeRelative * 100,
                High = ticker.High,
                Low = ticker.Low,
                Volume = ticker.Volume
            };

            return Json(new
            {
                data = model
            });
        }

        [HttpPost("CandlesTableGeneration")]
        public async Task<IActionResult> CandlesTableGeneration([FromBody] CandlesRequest request)
        {
            IEnumerable<CandleResponse> candles = await _restConnector.GetCandleSeriesAsync(request.Pair, request.Period, request.From, request.To, request.Count);

            List<CandleModel> models = new List<CandleModel>();
            foreach (CandleResponse candle in candles)
            {
                CandleModel model = new CandleModel()
                {
                    ClosePrice = candle.ClosePrice,
                    HighPrice = candle.HighPrice,
                    LowPrice = candle.LowPrice,
                    OpenPrice = candle.OpenPrice,
                    OpenTime = candle.OpenTime,
                    Pair = candle.Pair,
                    TotalPrice = candle.TotalPrice,
                    TotalVolume = candle.TotalVolume
                };
                models.Add(model);
            }
            return Json(new
            {
                data = models
            });
        }

        [HttpPost("TradesTableGeneration")]
        public async Task<IActionResult> TradesTableGeneration([FromBody] TradesRequest request)
        {
            IEnumerable<TradeResponse> trades = await _restConnector.GetNewTradesAsync(request.Pair, request.MaxCount);

            List<TradeModel> models = new List<TradeModel>();
            foreach (TradeResponse trade in trades)
            {
                TradeModel model = new TradeModel()
                {
                    Id = trade.Id,
                    Amount = trade.Amount,
                    Pair = trade.Pair,
                    Price = trade.Price,
                    Side = trade.Side,
                    Time = trade.Time,
                };
                models.Add(model);
            }
            return Json(new
            {
                data = models
            });
        }
    }
}
