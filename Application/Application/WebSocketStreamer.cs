using Confluent.Kafka;
using Domain;
using Infrastructure.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Socket
{
    public delegate void TradeReceivedHandler(CryptoTrade trade);
    public class WebSocketStreamer : IHostedService, IDisposable
    {
        private ClientWebSocket _webSocket = new ClientWebSocket();
        private readonly KafkaProducerService _producer;
        private string _kafkaTopic;
        public event TradeReceivedHandler OnTradeReceived;
        private readonly WebSocketConfig _webSocketConfig;
        private readonly ILogger<WebSocketStreamer> _logger;

        private DataTradeConfig _config;

        public WebSocketStreamer(
            KafkaProducerService kafkaProducerService,
            WebSocketConfig webSocketConfig,
            IOptions<KafkaSettings> kafkaSettings,
            ILogger<WebSocketStreamer> logger
            )
        {
            _producer = kafkaProducerService;
            _webSocketConfig = webSocketConfig;
            _kafkaTopic = kafkaSettings.Value.TransactionTopic;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
        //    StartStreamingAsync(new DataTradeConfig("Binance"));

        //    OnTradeReceived += OnTradeReceivedHandler;

            return Task.CompletedTask;
        }

        private void OnTradeReceivedHandler(CryptoTrade trade)
        {
            _producer.Produce(trade.ToJson(), _kafkaTopic);
        }

        public async Task StartStreamingAsync(DataTradeConfig config)
        {
            _config = config;
            while (true)
            {
                try
                {
                    _webSocket = new ClientWebSocket();

                    await _webSocket.ConnectAsync(_webSocketConfig.GetCompletedUri(), CancellationToken.None);
                    _logger.LogInformation("_webSocketConfig.GetCompletedUri() : " + _webSocketConfig.GetCompletedUri());
                    await ListenToWebSocketAsync();

                }
                catch (Exception e)
                {
                    _logger.LogInformation($"WebSocket Exception: {e.Message}");
                    await Task.Delay(5000); 
                }
            }
        }

        private async Task ListenToWebSocketAsync()
        {
            var buffer = new byte[1024 * 4];
            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var trade = JsonConvert.DeserializeObject<CryptoTrade>(message);
                trade.ExchangeName = _config.ExchangeName;

                OnTradeReceived?.Invoke(trade);

                _logger.LogInformation($"Received: {trade.ToJson()}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            OnTradeReceived -= OnTradeReceivedHandler;
            _producer.Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            OnTradeReceived -= OnTradeReceivedHandler;
        }
    }
}
