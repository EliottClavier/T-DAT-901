using Domain.Domain;

namespace Application
{
    public interface IExchangeScrappingInfoProvider
    {
        IEnumerable<ExchangeScrappingInfo>? GetExchangeScrappingInfo();
    }
}