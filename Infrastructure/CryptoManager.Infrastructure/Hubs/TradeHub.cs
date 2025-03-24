using Microsoft.AspNetCore.SignalR;

namespace CryptoManager.Infrastructure.Hubs
{
    public class TradeHub : Hub
    {
        public async Task SendTrade(string pair, string type, decimal amount, decimal price)
        {
            await Clients.All.SendAsync("ReceiveTrade", pair, type, amount, price);
        }
        public async Task SendCandle(string pair, decimal open, decimal close, decimal high, decimal low, decimal volume, DateTimeOffset openTime)
        {
            await Clients.All.SendAsync("ReceiveCandle", pair, open, close, high, low, volume, openTime);
        }
    }
}
