namespace Domain
{
    public interface ICryptoScraperService : IDisposable
    {
        Task<CryptoInfo> GetCryptoInfoAsync(string symbol);
        CryptoData GetCryptoInfoAsync();
        Task<CryptoInfo> GetCryptoInfoByApiAsync(string symbol);

    }

}