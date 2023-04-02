using System.Net.Http;
using NUnit.Framework;
using Split_Receipt.Payload;
using Split_Receipt.Services;

namespace Split_Receipt.Tests.Services
{
    [TestFixture]
    public class CurrencyServiceTests
    {
        private CurrencyService _currencyService;

        [SetUp]
        public void SetUp()
        {
            var httpClient = new HttpClient();
            _currencyService = new CurrencyService(httpClient);
        }

        [Test]
        public async Task GetLatestCurrencyData_AllParamsOk_ReturnsCurrencyResponse()
        {
            // Arrange
            string currencyBase = "USD";

            // Act
            var result = await _currencyService.GetLatestCurrencyData(currencyBase);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CurrencyResponse>(result);
        }

        [Test]
        public async Task GetLatestCurrencyData_NullCurrencyBase_ReturnsNull()
        {
            // Arrange
            string currencyBase = null;

            // Act
            var result = await _currencyService.GetLatestCurrencyData(currencyBase);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetLatestCurrencyData_NotExistingCurrency_ReturnsCurrencyResponseEuroBase()
        {
            // Arrange
            string currencyBase = "NotExistingCurrency";
            string expectedBase = "EUR";

            // Act
            var result = await _currencyService.GetLatestCurrencyData(currencyBase);
            string actualBase = result.Base;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedBase, actualBase);
        }

        [Test]
        public async Task GetRate_WithSameCurrencies_ReturnsOne()
        {
            // Arrange
            string currencyBase = "USD";
            string quoteCurrency = "USD";

            // Act
            var result = await _currencyService.GetRate(currencyBase, quoteCurrency);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public async Task GetRate_WithDifferentCurrencies_ReturnsExchangeRate()
        {
            // Arrange
            string currencyBase = "USD";
            string quoteCurrency = "EUR";

            // Act
            var result = await _currencyService.GetRate(currencyBase, quoteCurrency);

            // Assert
            Assert.Greater(result, 0);
        }
    }
}