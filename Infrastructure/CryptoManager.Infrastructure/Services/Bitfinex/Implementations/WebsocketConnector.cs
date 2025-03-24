using CryptoManager.Application.Common.Constants;
using CryptoManager.Application.Services.Interfaces;
using CryptoManager.Infrastructure.Options;
using CryptoManager.Infrastructure.Responses.Bitfinex;
using CryptoManager.Infrastructure.Services.Bitfinex.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;

namespace CryptoManager.Infrastructure.Services.Bitfinex.Implementations
{
    public class WebsocketConnector : IWebsocketConnector
    {
        private readonly ILogger<WebsocketConnector> _logger;
        private readonly IBitfinexService _bitfinex;

        private ClientWebSocket _webSocket;
        private CancellationTokenSource _cancellationTokenSource = new();
        private Dictionary<string, int> _subTrades = new Dictionary<string, int>();
        private Dictionary<string, int> _subCandles = new Dictionary<string, int>();

        public event Action<TradeResponse> NewBuyTrade;
        public event Action<TradeResponse> NewSellTrade;
        public event Action<CandleResponse> CandleSeriesProcessing;

        public WebsocketConnector(ILogger<WebsocketConnector> logger, IBitfinexService bitfinex)
        {
            _logger = logger;
            _bitfinex = bitfinex;
        }

        public async Task ConnectAsync()
        {
            _webSocket = new ClientWebSocket();
            try
            {
                await _webSocket.ConnectAsync(new Uri(_bitfinex.GetUrl(BitfinexOption.WSUrl)), _cancellationTokenSource.Token);
                _logger.LogInformation("WebSocket connected successfully.");
                _ = ReceiveMessages();
            }
            catch (Exception ex)
            {
                _logger.LogError($"WebSocket connection failed: {ex.Message}");
            }
        }

        public void SubscribeCandles(string pair, string period, DateTimeOffset? from = null, DateTimeOffset? to = null, long? count = 0)
        {
            string result = new StringBuilder($"t{pair}").ToString();
            if (!_subCandles.Keys.Any(x => x.EndsWith(result)))
            {
                var subscribeMessage = new
                {
                    @event = "subscribe",
                    channel = "candles",
                    key = $"trade:{period}:{result}"
                };

                SendMessage(subscribeMessage);
            }
        }
        public void SubscribeTrades(string pair, int maxCount = 100)
        {
            string result = new StringBuilder($"t{pair}").ToString();
            if (!_subTrades.ContainsKey(result))
            {
                var subscribeMessage = new
                {
                    @event = "subscribe",
                    channel = "trades",
                    symbol = $"{result}"
                };

                SendMessage(subscribeMessage);
            }
        }
        public void UnsubscribeCandles(string pair)
        {
            string result = new StringBuilder(pair).Insert(pair.LastIndexOf(":") + 1, "t").ToString();
            if (_subCandles.ContainsKey(result))
            {
                var unsubscribeMessage = new
                {
                    @event = "unsubscribe",
                    chanId = _subCandles[result]
                };

                SendMessage(unsubscribeMessage);
                _subCandles.Remove(result);
            }
        }
        public void UnsubscribeTrades(string pair)
        {
            string result = new StringBuilder($"t{pair}").ToString();
            if (_subTrades.ContainsKey(result))
            {
                var unsubscribeMessage = new
                {
                    @event = "unsubscribe",
                    chanId = _subTrades[result]
                };

                SendMessage(unsubscribeMessage);
                _subTrades.Remove(result);
            }
        }

        private async Task ReceiveMessages()
        {
            byte[] buffer = new byte[4096];
            StringBuilder messageBuilder = new StringBuilder();

            while (_webSocket.State == WebSocketState.Open)
            {
                try
                {
                    WebSocketReceiveResult result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

                        if (result.EndOfMessage)
                        {
                            string fullMessage = messageBuilder.ToString();
                            _logger.LogInformation($"Message received: {fullMessage}");
                            ProcessMessage(fullMessage);
                            messageBuilder.Clear();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error receiving message: {ex.Message}");
                    break;
                }
            }
        }

        private void ProcessMessage(string message)
        {
            try
            {
                JToken json = JToken.Parse(message);

                if (json.Type == JTokenType.Object)
                {
                    ProcessSubscription((JObject)json);
                }
                else if (json.Type == JTokenType.Array)
                {
                    ProcessMarketData((JArray)json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing message: {ex}");
            }
        }

        private void ProcessSubscription(JObject obj)
        {
            if (obj["event"]?.ToString() != "subscribed") return;

            string channel = obj.Value<string>("channel");
            string symbol = obj.Value<string>("symbol") == null ? obj.Value<string>("key") : obj.Value<string>("symbol");

            if (channel == null || (symbol == null))
            {
                _logger.LogError(" miss required fields.");
                return;
            }

            int chanId = obj["chanId"].ToObject<int>();

            if (channel == "trades")
                _subTrades[symbol] = chanId;
            else if (channel == "candles")
                _subCandles[symbol] = chanId;

            _logger.LogInformation($"Subscribed to {channel} for {symbol} with channel ID {chanId}");
        }

        private void ProcessMarketData(JArray array)
        {
            if (array.Count < 2) return;
            if (array.Last.ToString() == "hb") return;

            int channelId = array[0].ToObject<int>();
            JToken channelData = array[1];
            if (_subTrades.ContainsValue(channelId))
            {
                if (channelData.Type == JTokenType.String && channelData.ToString() == "te")
                {
                    ProcessTrade(array[2], channelId);
                }
                else if (channelData.Type == JTokenType.Array)
                {
                    foreach (JToken trade in (JArray)channelData)
                    {
                        ProcessTrade(trade, channelId);
                    }
                }
            }
            if (_subCandles.ContainsValue(channelId))
            {
                if (channelData[0].Type == JTokenType.Array)
                {
                    foreach (JToken item in (JArray)channelData)
                    {
                        ProcessCandle((JArray)item, channelId);
                    }
                }
                else
                {
                    ProcessCandle((JArray)channelData, channelId);
                }
            }
        }

        private void ProcessTrade(JToken tradeData, int channelId)
        {
            TradeResponse trade = new TradeResponse()
            {
                Id = tradeData[0].ToString(),
                Time = DateTimeOffset.FromUnixTimeMilliseconds(tradeData[1].ToObject<long>()),
                Amount = tradeData[2].ToObject<decimal>(),
                Price = tradeData[3].ToObject<decimal>(),
                Side = tradeData[2].ToObject<decimal>() > 0 ? TransactionParty.Buy : TransactionParty.Sell,
                Pair = _subTrades.FirstOrDefault(x => x.Value == channelId).Key.Substring(1)
            };

            if (trade.Side == TransactionParty.Buy)
                NewBuyTrade?.Invoke(trade);
            else
                NewSellTrade?.Invoke(trade);
        }

        private void ProcessCandle(JArray candleData, int channelId)
        {
            CandleResponse candle = new CandleResponse()
            {
                OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(candleData[0].ToObject<long>()),
                OpenPrice = candleData[1].ToObject<decimal>(),
                ClosePrice = candleData[2].ToObject<decimal>(),
                HighPrice = candleData[3].ToObject<decimal>(),
                LowPrice = candleData[4].ToObject<decimal>(),
                TotalVolume = candleData[5].ToObject<decimal>(),
                Pair = _subCandles.FirstOrDefault(x => x.Value == channelId).Key.Split(":")[2].Substring(1),
                TotalPrice = (candleData[3].ToObject<decimal>() + candleData[4].ToObject<decimal>()) / 2 * candleData[5].ToObject<decimal>(),
            };

            CandleSeriesProcessing?.Invoke(candle);
        }

        private async Task SendMessage(object message)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
        }
    }
}