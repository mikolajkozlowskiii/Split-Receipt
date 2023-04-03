using Split_Receipt.Payload;

namespace Split_Receipt.Services.Interfaces
{
    /// <summary>
    /// The interface <t>ICurrencyService</t> contains methods for performing
    /// operations on the <c>CurrencyResponse</c> class. The interface includes
    /// methods for retrieving the latest currency data and for getting the
    /// exchange rate between two currencies.
    /// </summary>
    public interface ICurrencyService
    {
        Task<CurrencyResponse> GetLatestCurrencyData(string currencyBase);
        Task<Decimal> GetRate(string currencyBase, string quoteCurrency);
    }
}
