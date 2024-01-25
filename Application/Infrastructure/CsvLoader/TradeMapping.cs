using CsvHelper.Configuration;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.CsvLoader
{
    public class TradeMapping : ClassMap<CryptoTrade>
    {
        public TradeMapping(string symbol)
        {
            Map(m => m.TradeId).Index(0);
            Map(m => m.Price).Index(1);
            Map(m => m.Quantity).Index(2);
            Map(m => m.TimeStamp).Index(5);
            Map(m => m.ExchangeName).Constant("Binance");
            Map(m => m.CurrencySymbol).Constant(symbol);
        }


    }
}
