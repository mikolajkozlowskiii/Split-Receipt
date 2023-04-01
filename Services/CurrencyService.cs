using Newtonsoft.Json;
using Split_Receipt.Payload;
using Split_Receipt.Services.Interfaces;
using System.Net;

namespace Split_Receipt.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// This method is used for consuming data from currency api.
        /// All values of rates are latest.
        /// </summary>
        /// <param name="currencyBase"></param> based on this are returned exchange rates.
        /// <returns>CurrencyResponse object instance with all latest rates.</returns>
        public async Task<CurrencyResponse> GetLatestCurrencyData(string currencyBase)
        {
            if(currencyBase == null)
            {
                return null;
            }
            var response = await _httpClient.GetAsync($"https://api.exchangerate.host/latest?base={currencyBase}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<CurrencyResponse>(content);
            }
            return null;
        }

        /// <summary>
        /// This method is used for getting rate of specific currency based on specific currency.
        /// </summary>
        /// <param name="currencyBase"></param> is a base of currency transferred to
        /// GetLatestCurrencyData method.
        /// <param name="quoteCurrency"></param> is a quote currency for which rate is extract
        /// from GetLatestCurrencyData method.
        /// <returns>If currency base and quote currency are the same it returns 1.
        /// Otherwise it returns answer from GetLatestCurrencyData, more precisely rate of
        /// that instance.</returns>
        public async Task<Decimal> GetRate(string currencyBase, string quoteCurrency)
        {
            if (quoteCurrency.Equals(currencyBase))
            {
                return 1;
            }
            var currencyData = await GetLatestCurrencyData(currencyBase);
            decimal value;
            currencyData.Rates.TryGetValue(quoteCurrency, out value);
            return value;
        }
    }
    
}
