using Newtonsoft.Json;
using Split_Receipt.Payload;
using Split_Receipt.Services.Interfaces;
using System.Net;
using System.Net.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
    }
    
}
