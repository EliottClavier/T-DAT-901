
using Domain;
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

            services.AddSingleton<Func<string, ICryptoScraperService>>(serviceProvider => key =>
            {
                var cmcLogger = serviceProvider.GetRequiredService<ILogger<CoinMarketCapCryptoScraperService>>();
                var binLogger = serviceProvider.GetRequiredService<ILogger<BinanceScraperService>>();

                switch (key)
                {
                    case "CMC-BTC":
                        return new CoinMarketCapCryptoScraperService(CurrencySymbol.Btc, "BTCUSD", Exchange.CoinMarketCap, "https://coinmarketcap.com/currencies/bitcoin/", cmcLogger);
                    case "CMC-ETH":
                        return new CoinMarketCapCryptoScraperService(CurrencySymbol.Eth, "ETHUSD", Exchange.CoinMarketCap, "https://coinmarketcap.com/currencies/ethereum", cmcLogger);
                    case "BNB-BTC":
                        return new BinanceScraperService(CurrencySymbol.Btc, "BTCUSD", Exchange.Binance, "https://www.binance.com/en/trade/BTC_USDT", binLogger);
                    case "BNB-ETH":
                        return new BinanceScraperService(CurrencySymbol.Eth, "ETHUSD", Exchange.Binance, "https://www.binance.com/en/trade/ETH_USDT", binLogger);
                    default:
                        throw new KeyNotFoundException(); 
                }
            });

            return services;
        }
    }


}
