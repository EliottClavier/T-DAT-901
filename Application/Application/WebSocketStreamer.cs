using Application;
using Domain;
using Infrastructure.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace Infrastructure.Socket
{
    public delegate void TradeReceivedHandler(CryptoTrade trade);
    public class WebSocketStreamer : IHostedService, IDisposable
    {
        private readonly KafkaProducerService _producer;
        private string _kafkaTopic;
        public event TradeReceivedHandler? OnTradeReceived;
        private readonly WebSocketConfig _webSocketConfig;
        private readonly ILogger<WebSocketStreamer> _logger;
        private int _reconnectAttempts = 0;
        private const int MaxReconnectAttempts = 5;
        private const int BaseReconnectDelayMs = 1000; // 1 seconde
        private const int MaxReconnectDelayMs = 60000; // 1 minute

        private DataTradeConfig? _config;

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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
        
            await StartStreamingAsync(new DataTradeConfig("Binance"), cancellationToken);

        }

        private void OnTradeReceivedHandler(CryptoTrade trade)
        {
            _producer.Produce(trade.ToJson(), _kafkaTopic);
            _logger.LogInformation($"Received: {trade.ToJson()}");
        }

        public async Task StartStreamingAsync(DataTradeConfig config, CancellationToken cancellationToken)
        {
            OnTradeReceived += OnTradeReceivedHandler;
            _config = config;
            while (!cancellationToken.IsCancellationRequested && _reconnectAttempts < MaxReconnectAttempts)
            {
                try
                {
                    using (var client = new ClientWebSocket())
                    {                      
                        await client.ConnectAsync(_webSocketConfig.GetCompletedUri(), cancellationToken);
                        _reconnectAttempts = 0;
                        await ListenToWebSocketAsync(client, cancellationToken);
                    }
                }
                catch (Exception e)
                {
                    _reconnectAttempts++;
                    _logger.LogInformation($"Tentative de reconnexion {_reconnectAttempts}/{MaxReconnectAttempts}, erreur : {e.Message}");
                    int delay = Math.Min(BaseReconnectDelayMs * (int)Math.Pow(2, _reconnectAttempts), MaxReconnectDelayMs);
                    await Task.Delay(delay, cancellationToken);
                  
                }
            }

            if (_reconnectAttempts >= MaxReconnectAttempts)
            {
                _logger.LogError("Nombre maximal de tentatives de reconnexion atteint.");
            }
        }

        private async Task ListenToWebSocketAsync(ClientWebSocket client, CancellationToken cancellationToken )
        {
            var buffer = new byte[1024 * 4];
            string message;
            CryptoTrade trade;
            while (client.State == WebSocketState.Open)
            {
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                trade = JsonConvert.DeserializeObject<CryptoTrade>(message);
                trade.ExchangeName = _config.ExchangeName;

                OnTradeReceived?.Invoke(trade);
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Dispose();
           
            
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            OnTradeReceived -= OnTradeReceivedHandler;
            _producer.Dispose();            
        }
    }
}
