using CryptoManager.Infrastructure.Responses.Bitfinex;
using CryptoManager.Infrastructure.Services.Bitfinex.Interfaces;
using CryptoManager.WebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CryptoManager.WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRestConnector _restConnector;

        public HomeController(IRestConnector restConnector) => _restConnector = restConnector;
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrencyPairs()
        {
            CurrencyResponse currency = await _restConnector.GetCurrencyAsync();

            ÑurrencyModel model = new ÑurrencyModel()
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
