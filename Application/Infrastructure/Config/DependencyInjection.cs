
using Domain;
using Domain.Domain;
using Domain.Ports;
using Infrastructure.Kafka;
using Infrastructure.Scraper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;



namespace Infrastructure.Config
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(new KafkaProducerService("localhost:9092"));
      

            services.AddSingleton<Func<ExchangeScrappingInfo, ICryptoScraperService>>(info =>
            {
                //var cmcLogger = serviceProvider.GetRequiredService<ILogger<CoinMarketCapCryptoScraperService>>();
                //var binLogger = serviceProvider.GetRequiredService<ILogger<BinanceScraperService>>();

                if (info.ExchangeName == Exchange.CoinMarketCap)
                {
                    switch (info.Currency)
                    {
                        case CurrencySymbol.Btc:
                            return new CoinMarketCapCryptoScraperService(info);
                        case CurrencySymbol.Eth:
                            return new CoinMarketCapCryptoScraperService(info);
                        default:
                            throw new KeyNotFoundException();
                    }
                }
                else if (info.ExchangeName == Exchange.Binance)
                {
                    switch (info.Currency)
                    {
                        case CurrencySymbol.Btc:
                            return new BinanceScraperService(info);
                        case CurrencySymbol.Eth:
                            return new BinanceScraperService(info);
                        // Ajoutez d'autres cas au besoin
                        default:
                            throw new KeyNotFoundException();
                    }
                }
                else
                {
                    throw new KeyNotFoundException();
                }

                //    switch (info)
                //    {
                //        case "CMC-BTC":
                //            return new CoinMarketCapCryptoScraperService(CurrencySymbol.Btc, "BTCUSD", Exchange.CoinMarketCap, "https://coinmarketcap.com/currencies/bitcoin/", cmcLogger);
                //        case "CMC-ETH":
                //            return new CoinMarketCapCryptoScraperService(CurrencySymbol.Eth, "ETHUSD", Exchange.CoinMarketCap, "https://coinmarketcap.com/currencies/ethereum", cmcLogger);
                //        case "BNB-BTC":
                //            return new BinanceScraperService(CurrencySymbol.Btc, "BTCUSD", Exchange.Binance, "https://www.binance.com/en/markets/overview", binLogger);
                //        case "BNB-ETH":
                //            return new BinanceScraperService(CurrencySymbol.Eth, "ETHUSD", Exchange.Binance, "https://www.binance.com/en/markets/overview", binLogger);
                //        case "BNB-ADA":
                //            return new BinanceScraperService(CurrencySymbol.Ada, "ADAUSD", Exchange.Binance, "https://www.binance.com/en/markets/overview", binLogger);
                //        case "BNB-XRP":
                //            return new BinanceScraperService(CurrencySymbol.Xrp, "XRPUSD", Exchange.Binance, "https://www.binance.com/en/markets/overview", binLogger);
                //        default:
                //            throw new KeyNotFoundException(); 
                //    }
             

         
            });
            return services;
        }

    }
}
