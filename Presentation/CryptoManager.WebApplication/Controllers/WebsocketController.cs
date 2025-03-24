using CryptoManager.Infrastructure.Services.Bitfinex.Interfaces;
using CryptoManager.WebApplication.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CryptoManager.WebApplication.Controllers
{
    [Route("[controller]")]
    public class WebsocketController : Controller
    {
        private readonly IWebsocketConnector _websocketConnector;

        public WebsocketController(IWebsocketConnector websocketConnector) => _websocketConnector = websocketConnector;
        
        [HttpPost("SubscribeTrades")]
        public async Task<IActionResult> SubscribeTrades([FromBody] TradesRequest request)
        {
            try
            {
                _websocketConnector.SubscribeTrades(request.Pair);
                return Json(new { success = true, message = "Subscribe Trades" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("SubscribeCandles")]
        public async Task<IActionResult> SubscribeCandles([FromBody]CandlesRequest request)
        {
            try
            {
                _websocketConnector.SubscribeCandles(request.Pair, request.Period);
                return Json(new { success = true, message = "Subscribe Candles" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("UnsubscribeTrades")]
        public async Task<IActionResult> UnsubscribeTrades([FromBody] TradesRequest request)
        {
            try
            {
                _websocketConnector.UnsubscribeTrades(request.Pair);
                return Json(new { success = true, message = "Unsubscribe Trades" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("UnsubscribeCandles")]
        public async Task<IActionResult> UnsubscribeCandles([FromBody]CandlesRequest request)
        {
            try
            {
                _websocketConnector.UnsubscribeCandles($"trade:{request.Period}:{request.Pair}");
                return Json(new { success = true, message = "Unsubscribe Candles" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }      
    }
}
