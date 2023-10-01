using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class CryptoData
    {
        public string? Name { get; set; }
        public string? Symbol { get; set; }
        //public decimal Price { get; set; }
        public string? Price { get; set; }
        //public decimal Volume24H { get; set; }
        public string? Volume24H { get; set; }
        //public decimal CirculatingSupply { get; set; }
        public string? CirculatingSupply { get; set; }
        public long TimeStamp { get; set; }
      
    }
}
