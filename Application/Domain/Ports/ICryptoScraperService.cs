namespace Domain.Ports
{
    public interface ICryptoScraperService : IDisposable
    {
        CryptoData? GetCryptoInfoAsync();
    }

}