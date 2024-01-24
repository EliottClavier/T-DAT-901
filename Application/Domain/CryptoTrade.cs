using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class CryptoTrade
    {
        [JsonProperty("p")]
        public string? Price { get; set; }

        [JsonProperty("q")]
        public string? Quantity { get; set; }

        [JsonProperty("s")]
        public string? CurrencySymbol { get; set; }

        [JsonProperty("T")]
        public long? TimeStamp { get; set; }

        [JsonProperty("a")]
        public string? TradeId { get; set; }

        public string? ExchangeName { get; set; }

        public CryptoTrade(string exchangeName)
        {
            ExchangeName = exchangeName;
        }
        public string ToJson()
        {
            return $"{{\"TradeId\": \"{TradeId}\", \"Price\": \"{Price}\", \"Quantity\": \"{Quantity}\", \"CurrencySymbol\": \"{CurrencySymbol}\", \"TimeStamp\": {TimeStamp}, \"ExchangeName\": \"{ExchangeName}\" }}";
        }
    }
}
