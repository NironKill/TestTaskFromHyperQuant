using CryptoManager.Infrastructure.Services.Bitfinex.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CryptoManager.WebApplication.Controllers
{
    [Route("[controller]")]
    public class WebsocketController : Controller
    {
        private readonly IWebsocketConnector _websocketConnector;

        public WebsocketController(IWebsocketConnector websocketConnector) => _websocketConnector = websocketConnector;
    }
}
