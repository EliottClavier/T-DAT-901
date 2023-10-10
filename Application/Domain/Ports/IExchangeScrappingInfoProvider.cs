using Domain.Domain;

namespace Domain.Ports
{
    public interface IExchangeScrappingInfoProvider
    {
        IEnumerable<ExchangeScrappingInfo>? GetExchangeScrappingInfo();
    }
}