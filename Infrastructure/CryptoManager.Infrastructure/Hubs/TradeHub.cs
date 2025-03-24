using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace CryptoManager.Infrastructure.Hubs
{
    public class TradeHub : Hub
    {
        public async Task SendTrade(string id, string pair, string side, decimal price, decimal amount, DateTimeOffset time) =>     
            await Clients.All.SendAsync("ReceiveTrade", id, pair, side, price, amount, time);
        
        public async Task SendCandle(string pair, decimal open, decimal close, decimal high, decimal low, decimal volume, DateTimeOffset openTime) => 
            await Clients.All.SendAsync("ReceiveCandle", pair, open, close, high, low, volume, openTime);
    }
}
