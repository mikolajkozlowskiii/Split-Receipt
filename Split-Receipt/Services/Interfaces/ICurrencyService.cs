using Split_Receipt.Payload;

namespace Split_Receipt.Services.Interfaces
{
    /// <summary>
    /// Interface <t>ICurrencyService</t> that contains methods for operation on
    /// <c>CurrencyResponse</c>.
    /// </summary>
    public interface ICurrencyService
    {
        Task<CurrencyResponse> GetLatestCurrencyData(string currencyBase);
        Task<Decimal> GetRate(string currencyBase, string quoteCurrency);
    }
}
