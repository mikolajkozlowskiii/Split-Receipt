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

        public void Get(string currencyBase)
        {
            var url = $"https://api.exchangerate.host/latest?base=USD";

            var web = new WebClient();

            var response = web.DownloadString(url);
        }
        public async Task<CurrencyResponse> GetCurrencyData(string currencyBase)
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
            var currencyData = await GetCurrencyData(currencyBase);
            decimal value;
            currencyData.Rates.TryGetValue(quoteCurrency, out value);
            return value;
        }
    }
    
}
