﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Domain
    {
        public class ExchangeScrappingInfo
        {
            public string ExchangeName { get; set; }
            public string Currency { get; set; }

            public string Url { get; set; }

            public string CurrencySymbol { get; set; }

            public string CurrencyPair { get; set; }

            public string PriceXPath { get; set; }
            public string Volume24HXPath { get; set; }
            public string CirculatingSupplyXPath { get; set; }




        }

    }
}