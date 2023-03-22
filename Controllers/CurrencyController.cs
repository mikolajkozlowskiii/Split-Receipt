using Microsoft.AspNetCore.Mvc;
using Split_Receipt.Payload;
using Split_Receipt.Services.Interfaces;

namespace Split_Receipt.Controllers
{
    public class CurrencyController : Controller
    {

        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public async Task<IActionResult> Index()
        {
            //_currencyService.Get("pln");
            CurrencyResponse currencyData = await _currencyService.GetCurrencyData("PLN");
            if(currencyData != null)
            {
                decimal value;
                currencyData.Rates.TryGetValue("EUR", out value);
                Console.WriteLine(value);
            }
            return View();
        }
    }
}
// https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies/eur.json