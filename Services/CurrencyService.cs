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
        /// </summary>
        /// <param name="currencyBase"></param> based on this are returned exchange rates.
        /// <returns>CurrencyResponse object instance with all rates </returns>
        public async Task<CurrencyResponse> GetLatestCurrencyData(string currencyBase)
        {
            var response = await _httpClient.GetAsync($"https://api.exchangerate.host/latest?base={currencyBase}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<CurrencyResponse>(content);
            }
            return null;
        }
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
