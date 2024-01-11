using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class DataTradeConfig
    {
        public string ExchangeName { get; set; }
        public string CurrencySymbol { get; set; }
        public string KafkaTopic { get; set; }

        public DataTradeConfig(string ExchangName)
        {
            this.ExchangeName = ExchangName;
        }
    }
    
}
