using Split_Receipt.Payload;

namespace Split_Receipt.Services.Interfaces
{
    public interface ICurrencyService
    {
        void Get(string currencyBase);
        Task<CurrencyResponse> GetCurrencyData(string currencyBase);
        Task<Decimal> GetRate(string currencyBase, string quoteCurrency);
    }
}
