using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class CryptoData
    {
        public string? CurrencyName { get; set; }
        public string? CurrencyPair { get; set; }
        public string? CurrencySymbol { get; set; }

        public string? ExchangeName { get; set; }
   
        public string? Price { get; set; }
       
        public string? Volume24H { get; set; }
    
        public string? CirculatingSupply { get; set; }
        public string? Liquidity { get; set; } //Une mesure de la facilité avec laquelle la paire peut être achetée ou vendue sans affecter le prix du marché
        public string? TimeStamp { get; set; }


        public CryptoData()
        {
        }
        public CryptoData(
            string? currencyName = null, string? currencyPair = null, string? currencySymbol = null,
            string? exchangeName = null, string? price = null, string? volume24H = null,
            string? circulatingSupply = null, string? liquidity = null)
        {
            this.CurrencyName = currencyName;
            this.CurrencyPair = currencyPair;
            this.CurrencySymbol = currencySymbol;
            this.ExchangeName = exchangeName;
            this.Price = price;
            this.Volume24H = volume24H;
            this.CirculatingSupply = circulatingSupply;
            this.Liquidity = liquidity;
            this.TimeStamp = TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        }   
        

     
   
        

    }
}
