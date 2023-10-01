using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class CryptoData
    {
        public List<string> Name { get; set; } = new List<string>();
        public List<string> MarketCap { get; set; } = new List<string>();
        public List<string> Price { get; set; } = new List<string>();
        public List<string> CirculatingSupply { get; set; } = new List<string>();
        public List<string> Symbol { get; set; } = new List<string>();
    }
}
