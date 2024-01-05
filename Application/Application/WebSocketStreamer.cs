﻿using Confluent.Kafka;
using Domain;
using Infrastructure.Kafka;
using Microsoft.Extensions.Hosting;
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
        private readonly string _kafkaTopic = "crypto-transactions";
        public event TradeReceivedHandler OnTradeReceived;
        private readonly WebSocketConfig _webSocketConfig;

        private DataTradeConfig _config;

        public WebSocketStreamer(KafkaProducerService kafkaProducerService, WebSocketConfig webSocketConfig)
        {
            _producer = kafkaProducerService;
            _webSocketConfig = webSocketConfig;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            StartStreamingAsync(new DataTradeConfig("Binance"));

            OnTradeReceived += OnTradeReceivedHandler;
      

            return Task.CompletedTask;
        }

        private void OnTradeReceivedHandler(CryptoTrade trade)
        {           
           //_producer.Produce(_kafkaTopic, trade.ToJson());         
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
                    Console.WriteLine("_webSocketConfig.GetCompletedUri() : " + _webSocketConfig.GetCompletedUri());
                        await ListenToWebSocketAsync();
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine($"WebSocket Exception: {e.Message}");
                    await Task.Delay(5000); // Attendre avant de retenter la connexion
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

                Console.WriteLine($"Received: {trade.ToJson()}");               
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
