using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Split_Receipt.Areas.Identity.Data;
using Split_Receipt.Data;
using Split_Receipt.Models;
using Split_Receipt.Payload;
using Split_Receipt.Services.Interfaces;
using Split_Receipt.Services.Mappers;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Azure.Core;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Split_Receipt.Constants;

namespace Split_Receipt.Services.Tests
{
    [TestFixture]
    public class CheckoutServiceTests
    {
        private AuthDbContext _appContext;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private ICheckoutService _checkoutService;
        private CheckoutMapper _checkoutMapper;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AuthDbContext>()
               .UseInMemoryDatabase(databaseName: "SplitReceipt")
               .Options;

            _appContext = new AuthDbContext(options);

            var store = new Mock<IUserStore<ApplicationUser>>();
            var optionsMock = new Mock<IOptions<IdentityOptions>>();

            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, optionsMock.Object, new PasswordHasher<ApplicationUser>(),
                new IUserValidator<ApplicationUser>[0], new IPasswordValidator<ApplicationUser>[0],
                new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(),
                new ServiceCollection().BuildServiceProvider(),
                new Logger<UserManager<ApplicationUser>>(new LoggerFactory()));

            _checkoutMapper = new CheckoutMapper(_userManagerMock.Object);

            var _currencyServiceMock = new Mock<ICurrencyService>();
            _currencyServiceMock
                .Setup(x => x.GetRate("PLN", "EUR"))
                .ReturnsAsync(1 / 4.7m);
            _currencyServiceMock
                .Setup(x => x.GetRate("PLN", "PLN"))
                .ReturnsAsync(1.0m);
            _currencyServiceMock
                .Setup(x => x.GetRate("PLN", "GBP"))
                .ReturnsAsync(1 / 5.0m);
            _currencyServiceMock
               .Setup(x => x.GetRate("EUR", "PLN"))
               .ReturnsAsync(4.7m);
            _currencyServiceMock
                .Setup(x => x.GetRate("GBP", "PLN"))
                .ReturnsAsync(5.0m);


            _checkoutService = new CheckoutService(_appContext, _userManagerMock.Object,
                new GroupService(_appContext, _userManagerMock.Object, new GroupMapper(_appContext, _userManagerMock.Object)),
                _currencyServiceMock.Object, _checkoutMapper);
        }

        [TearDown]
        public async Task Cleanup()
        {
            _appContext.Database.EnsureDeleted();
        }

        [Test]
        public async Task Save_AllParamsOk_CheckoutSavedInDB()
        {  
            // Arrange
            var checkoutRequest = new CheckoutRequest {
                Currency = "EUR",
                Description = "Test",
                Price = 100,
                IsSplitted = true};
            var userId = "userId";
            var groupId = 1;

            // Act
            var actual = await _checkoutService.Save(checkoutRequest, userId, groupId);

            // Assert
            Assert.Greater(actual, 0);
        }

        [Test]
        public async Task Save_ParamsHaveNotRequiredFields_ExceptionThrown()
        {
            // Arrange
            var checkoutRequest = new CheckoutRequest
            {
                Currency = null,
                Description = null,
                Price = 0,
                IsSplitted = false
            };
            var userId = "userId";
            var groupId = 1;

            // Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _checkoutService.Save(checkoutRequest, userId, groupId));
        }

        [Test]
        public void Update_AllParamsOk_CheckoutUpdated()
        {
            // Arrange
            var checkout = new Checkout
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = true
            };
            _appContext.Checkouts.Add(checkout);
            _appContext.SaveChanges();

            var updateRequest = new CheckoutRequest {
                Currency = "EUR",
                Description = "Test",
                Price = 100,
                IsSplitted = true
            };
            var checkoutId = checkout.Id;

            // Act
            var actual = _checkoutService.Update(updateRequest, checkoutId);

            // Assert
            Assert.Greater(actual, 0);
        }

        [Test]
        public void Update_ChectkoutIdNotInDB_CheckoutNotUpdated()
        {
            // Arrange
            var updateRequest = new CheckoutRequest
            {
                Currency = "EUR",
                Description = "Test",
                Price = 100,
                IsSplitted = true
            };
            var checkoutId = 1;

            // Assert
            Assert.Throws<InvalidOperationException>(() => _checkoutService.Update(updateRequest, checkoutId));
        }

        [Test]
        public void Delete_AllParamsOk_CheckoutDeleted()
        {
            // Arrange
            var checkout = new Checkout
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = true
            };
            _appContext.Checkouts.Add(checkout);
            _appContext.SaveChanges();
            var checkoutId = checkout.Id;

            // Act
            var actual = _checkoutService.Delete(checkoutId);

            // Assert
            Assert.Greater(actual, 0);
        }

        [Test]
        public void Delete_ChectkoutIdNotInDB_CheckoutDeleted()
        {
            // Assert
            Assert.Throws<InvalidOperationException>(() => _checkoutService.Delete(1));
        }

        [Test]
        public async Task FindById_AllParamsOk_ReturnsCheckoutResponse()
        {
            // Arrange
            string userEmail = "test1@gmail.com";
            string userId = "123";
            var user = new ApplicationUser { Id = userId, UserName = userEmail, Email = userEmail, FirstName = "John", LastName = "Doe" };
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            var checkout = new Checkout
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 1,
                UserId = user.Id,
                IsSplitted = true
            };
            _appContext.Checkouts.Add(checkout);
            _appContext.SaveChanges();
            var checkoutId = checkout.Id;

            CheckoutResponse expectedResponse = new CheckoutResponse {
                CheckoutId = checkoutId,
                Currency = checkout.Currency,
                Description = checkout.Description,
                Price = checkout.Price,
                CreatedAt = checkout.CreatedAt,
                GroupId = checkout.GroupId,
                UserId = checkout.UserId,
                IsSplitted = checkout.IsSplitted,
                UserEmail = userEmail
            };

            // Act
            CheckoutResponse actualResponse = await _checkoutService.FindById(checkoutId);

            // Assert
            Assert.AreEqual(expectedResponse, actualResponse);
        } 

        [Test]
        public async Task FindById_ChectkoutIdNotInDB_ExceptionThrown()
        {
            // Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _checkoutService.FindById(1));
        }

        [Test]
        public async Task FindAll_AllParamsOk_ReturnsCheckoutResponse()
        {
            // Arrange
            string userEmail = "test1@gmail.com";
            string userId = "123";
            var user = new ApplicationUser { Id = userId, UserName = userEmail, Email = userEmail, FirstName = "John", LastName = "Doe" };
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            var checkout1 = new Checkout
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 1,
                UserId = user.Id,
                IsSplitted = true
            };

            var checkout2 = new Checkout
            {
                Currency = "USD",
                Price = 300,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 1,
                UserId = user.Id,
                IsSplitted = false
            };

            var checkout3 = new Checkout
            {
                Currency = "PLN",
                Price = 500,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 3,
                UserId = user.Id,
                IsSplitted = true
            };

            var checkoutList = new List<Checkout> { checkout1, checkout2, checkout3 };
            _appContext.Checkouts.AddRange(checkoutList);
            _appContext.SaveChanges();
            var expectedList = await _checkoutMapper.map(checkoutList);

            // Act
            var actualList = await _checkoutService.FindAll();

            // Assert
            CollectionAssert.AreEquivalent(expectedList, actualList);
        }

        [Test]
        public async Task FindAll_NoCheckoutsInDB_ReturnsEmptyList()
        {
            // Arrange
            var expectedList = new List<CheckoutResponse>();

            // Act
            var actualList = await _checkoutService.FindAll();

            // Assert
            CollectionAssert.AreEquivalent(expectedList, actualList);
        }

        [Test]
         public async Task FindAllByGroupId_AllParamsOk_ReturnsListOfCheckoutResponse()
        {
            // Arrange
            string userEmail = "test1@gmail.com";
            string userId = "123";
            var user = new ApplicationUser { Id = userId, UserName = userEmail, Email = userEmail, FirstName = "John", LastName = "Doe" };
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            int groupId = 1;
            int diffrentGroupId = groupId + 1000;
            var checkout1 = new Checkout
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = groupId,
                UserId = user.Id,
                IsSplitted = true
            };

            var checkout2 = new Checkout
            {
                Currency = "USD",
                Price = 300,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = groupId,
                UserId = user.Id,
                IsSplitted = false
            };

            var checkout3 = new Checkout
            {
                Currency = "PLN",
                Price = 500,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = diffrentGroupId,
                UserId = user.Id,
                IsSplitted = true
            };

            var checkoutList = new List<Checkout> { checkout1, checkout2, checkout3 };
            _appContext.Checkouts.AddRange(checkoutList);
            _appContext.SaveChanges();
            var expectedList = await _checkoutMapper.map(new List<Checkout> { checkout1, checkout2 });

            // Act
            var actualList = await _checkoutService.FindlAllByGroupId(groupId, "Price ASC");

            // Assert
            CollectionAssert.AreEquivalent(expectedList, actualList); 
        }

        [Test]
        public async Task FindAllByGroupId_NoCheckoutsInDB_ReturnsEmptyList()
        {
            // Arrange
            var expectedList = new List<CheckoutResponse>();

            // Act
            var actualList = await _checkoutService.FindlAllByGroupId(1, "Price ASC");

            // Assert
            CollectionAssert.AreEquivalent(expectedList, actualList);
        }

        [Test]
        public async Task FindAllByUserId_AllParamsOk_ReturnsListOfCheckoutResponse()
        {
            // Arrange
            string userEmail = "test1@gmail.com";
            string userId = "123";
            string diffrentUserId = userId + "diffrent";
            var user = new ApplicationUser { Id = userId, UserName = userEmail, Email = userEmail, FirstName = "John", LastName = "Doe" };
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            int groupId = 1;
            var checkout1 = new Checkout
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = groupId,
                UserId = user.Id,
                IsSplitted = true
            };

            var checkout2 = new Checkout
            {
                Currency = "USD",
                Price = 300,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = groupId,
                UserId = diffrentUserId,
                IsSplitted = false
            };

            var checkout3 = new Checkout
            {
                Currency = "PLN",
                Price = 500,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = groupId,
                UserId = user.Id,
                IsSplitted = true
            };

            var checkoutList = new List<Checkout> { checkout1, checkout2, checkout3 };
            _appContext.Checkouts.AddRange(checkoutList);
            _appContext.SaveChanges();
            var expectedList = await _checkoutMapper.map(new List<Checkout> { checkout1, checkout3 });

            // Act
            var actualList = await _checkoutService.FindAllByUserID(user.Id);

            // Assert
            CollectionAssert.AreEquivalent(expectedList, actualList);
        }

        [Test]
        public async Task FindAllByUserId_NoCheckoutsInDB_ReturnsEmptyList()
        {
            // Arrange  
            var expectedList = new List<CheckoutResponse>();

            // Act
            var actualList = await _checkoutService.FindAllByUserID("123");

            // Assert
            CollectionAssert.AreEquivalent(expectedList, actualList);
        }

        [Test]
        public async Task CheckIsUserInCheckout_UserIsInCheckout_ReturnsTrue()
        {
            // Arrange
            var userId = "userId";
            var checkout = new Checkout
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 1,
                UserId = userId,
                IsSplitted = true
            };
            _appContext.Checkouts.Add(checkout);
            _appContext.SaveChanges();

            var checkoutId = checkout.Id;
            bool expectedResult = true;

            // Act
            bool actualResult = _checkoutService.CheckIsUserInCheckout(userId, checkoutId);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public async Task CheckIsUserInCheckout_UserIsNotInCheckout_ReturnsTFalse()
        {
            // Arrange
            var userId = "userId";
            var diffrentUserId = userId + "diffrent";
            var checkout = new Checkout
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 1,
                UserId = userId,
                IsSplitted = true
            };
            _appContext.Checkouts.Add(checkout);
            _appContext.SaveChanges();

            var checkoutId = checkout.Id;
            bool expectedResult = false;

            // Act
            bool actualResult = _checkoutService.CheckIsUserInCheckout(diffrentUserId, checkoutId);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public async Task SortByPricedDesc_SameCurrency_SortedCheckoutResponses()
        {
            // Arrange
            string sameCurrencyForAllCheckouts = "PLN";
            decimal priceCheckout1 = 100;
            decimal priceCheckout2 = 500;
            decimal priceCheckout3 = 300;
            string sortBy = SortOption.PriceDesc.GetDescription();
            
            var checkout1 = new CheckoutResponse
            {
                Currency = sameCurrencyForAllCheckouts,
                Price = priceCheckout1,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = true,
                UserEmail = "test",
                CheckoutId = 1
            };

            var checkout2 = new CheckoutResponse
            {
                Currency = sameCurrencyForAllCheckouts,
                Price = priceCheckout2,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = false,
                UserEmail = "test2",
                CheckoutId = 2
            };

            var checkout3 = new CheckoutResponse
            {
                Currency = sameCurrencyForAllCheckouts,
                Price = priceCheckout3,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 3,
                UserId = "userId",
                IsSplitted = true,
                UserEmail = "test3",
                CheckoutId = 3
            };

            var beforeSort = new List<CheckoutResponse> { checkout1, checkout2, checkout3 };
            var expectedAfterSortList = new List<CheckoutResponse> { checkout2, checkout3, checkout1};

            // Act
            var actualAfterSortList = await _checkoutService.Sort(sortBy, beforeSort);

            // Assert
            Assert.AreEqual(expectedAfterSortList, actualAfterSortList);
        }

        [Test]
        public async Task SortByPricedDesc_DiffrentCurrencies_SortedCheckoutResponses()
        {
            // Arrange           
            decimal priceCheckout1 = 100;
            string currencyCheckout1 = "GBP";
            decimal priceCheckout2 = 101;
            string currencyCheckout2 = "EUR";
            decimal priceCheckout3 = 200;
            string currencyCheckout3 = "PLN";

            string sortBy = SortOption.PriceDesc.GetDescription();

            var checkout1 = new CheckoutResponse
            {
                Currency = currencyCheckout1,
                Price = priceCheckout1,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = true,
                UserEmail = "test",
                CheckoutId = 1
            };

            var checkout2 = new CheckoutResponse
            {
                Currency = currencyCheckout2,
                Price = priceCheckout2,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = false,
                UserEmail = "test2",
                CheckoutId = 2
            };

            var checkout3 = new CheckoutResponse
            {
                Currency = currencyCheckout3,
                Price = priceCheckout3,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 3,
                UserId = "userId",
                IsSplitted = true,
                UserEmail = "test3",
                CheckoutId = 3
            };

            var beforeSort = new List<CheckoutResponse> { checkout1, checkout2, checkout3 };
            var expectedAfterSortList = new List<CheckoutResponse> { checkout1, checkout2, checkout3 };

            // Act
            var actualAfterSortList = await _checkoutService.Sort(sortBy, beforeSort);

            // Assert
            Assert.AreEqual(expectedAfterSortList, actualAfterSortList);
        }

        [Test]
        public async Task SortByPricedDesc_EmptyListReturnsEmptyList()
        {
            // Arrange
            string sortBy = SortOption.PriceDesc.GetDescription();
            var expectedAfterSortList = new List<CheckoutResponse>();

            // Act
            var actualAfterSortList = await _checkoutService.Sort(sortBy, expectedAfterSortList);

            // Assert
            Assert.AreEqual(expectedAfterSortList, actualAfterSortList);
        }

        [Test]
        public async Task SortByPricedAsc_SameCurrency_SortedCheckoutResponses()
        {
            // Arrange
            string sameCurrencyForAllCheckouts = "PLN";
            decimal priceCheckout1 = 100;
            decimal priceCheckout2 = 500;
            decimal priceCheckout3 = 300;
            string sortBy = SortOption.PriceAsc.GetDescription();

            var checkout1 = new CheckoutResponse
            {
                Currency = sameCurrencyForAllCheckouts,
                Price = priceCheckout1,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = true,
                UserEmail = "test",
                CheckoutId = 1
            };

            var checkout2 = new CheckoutResponse
            {
                Currency = sameCurrencyForAllCheckouts,
                Price = priceCheckout2,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = false,
                UserEmail = "test2",
                CheckoutId = 2
            };

            var checkout3 = new CheckoutResponse
            {
                Currency = sameCurrencyForAllCheckouts,
                Price = priceCheckout3,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 3,
                UserId = "userId",
                IsSplitted = true,
                UserEmail = "test3",
                CheckoutId = 3
            };

            var beforeSort = new List<CheckoutResponse> { checkout1, checkout2, checkout3 };
            var expectedAfterSortList = new List<CheckoutResponse> { checkout1, checkout3, checkout2 };

            // Act
            var actualAfterSortList = await _checkoutService.Sort(sortBy, beforeSort);

            // Assert
            Assert.AreEqual(expectedAfterSortList, actualAfterSortList);
        }

        [Test]
        public async Task SortByPricedAsc_DiffrentCurrencies_SortedCheckoutResponses()
        {
            // Arrange           
            decimal priceCheckout1 = 100;
            string currencyCheckout1 = "GBP";
            decimal priceCheckout2 = 101;
            string currencyCheckout2 = "EUR";
            decimal priceCheckout3 = 200;
            string currencyCheckout3 = "PLN";

            string sortBy = SortOption.PriceAsc.GetDescription();

            var checkout1 = new CheckoutResponse
            {
                Currency = currencyCheckout1,
                Price = priceCheckout1,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = true,
                UserEmail = "test",
                CheckoutId = 1
            };

            var checkout2 = new CheckoutResponse
            {
                Currency = currencyCheckout2,
                Price = priceCheckout2,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = false,
                UserEmail = "test2",
                CheckoutId = 2
            };

            var checkout3 = new CheckoutResponse
            {
                Currency = currencyCheckout3,
                Price = priceCheckout3,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 3,
                UserId = "userId",
                IsSplitted = true,
                UserEmail = "test3",
                CheckoutId = 3
            };

            var beforeSort = new List<CheckoutResponse> { checkout1, checkout2, checkout3 };
            var expectedAfterSortList = new List<CheckoutResponse> { checkout3, checkout2, checkout1 };

            // Act
            var actualAfterSortList = await _checkoutService.Sort(sortBy, beforeSort);

            // Assert
            Assert.AreEqual(expectedAfterSortList, actualAfterSortList);
        }

        [Test]
        public async Task SortByPricedAsc_EmptyListReturnsEmptyList()
        {
            // Arrange
            string sortBy = SortOption.PriceAsc.GetDescription();
            var expectedAfterSortList = new List<CheckoutResponse>();

            // Act
            var actualAfterSortList = await _checkoutService.Sort(sortBy, expectedAfterSortList);

            // Assert
            Assert.AreEqual(expectedAfterSortList, actualAfterSortList);
        }

        [Test]
        public async Task SortByDateAsc_DiffrentYears_SortedCheckoutResponses()
        {
            // Arrange
            DateTime dateCheckout1 = new DateTime(2010, 2, 9, 8, 30, 2);
            DateTime dateCheckout2 = new DateTime(2020, 2, 9, 8, 30, 2);
            DateTime dateCheckout3 = new DateTime(2015, 2, 9, 8, 30, 2);
            string sortBy = SortOption.DateAsc.GetDescription();

            var checkout1 = new CheckoutResponse
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = dateCheckout1,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = true,
                UserEmail = "test",
                CheckoutId = 1
            };

            var checkout2 = new CheckoutResponse
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = dateCheckout2,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = false,
                UserEmail = "test2",
                CheckoutId = 2
            };

            var checkout3 = new CheckoutResponse
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = dateCheckout3,
                Description = "test",
                GroupId = 3,
                UserId = "userId",
                IsSplitted = true,
                UserEmail = "test3",
                CheckoutId = 3
            };

            var beforeSort = new List<CheckoutResponse> { checkout1, checkout2, checkout3 };
            var expectedAfterSortList = new List<CheckoutResponse> { checkout1, checkout3, checkout2 };

            // Act
            var actualAfterSortList = await _checkoutService.Sort(sortBy, beforeSort);

            // Assert
            Assert.AreEqual(expectedAfterSortList, actualAfterSortList);
        }

        [Test]
        public async Task SortByDateAsc_SameYearsDiffrentMonths_SortedCheckoutResponses()
        {
            // Arrange
            DateTime dateCheckout1 = new DateTime(2010, 11, 9, 8, 30, 2);
            DateTime dateCheckout2 = new DateTime(2010, 10, 9, 8, 30, 2);
            DateTime dateCheckout3 = new DateTime(2010, 5, 9, 8, 30, 2);
            string sortBy = SortOption.DateAsc.GetDescription();

            var checkout1 = new CheckoutResponse
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = dateCheckout1,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = true,
                UserEmail = "test",
                CheckoutId = 1
            };

            var checkout2 = new CheckoutResponse
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = dateCheckout2,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = false,
                UserEmail = "test2",
                CheckoutId = 2
            };

            var checkout3 = new CheckoutResponse
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = dateCheckout3,
                Description = "test",
                GroupId = 3,
                UserId = "userId",
                IsSplitted = true,
                UserEmail = "test3",
                CheckoutId = 3
            };

            var beforeSort = new List<CheckoutResponse> { checkout1, checkout2, checkout3 };
            var expectedAfterSortList = new List<CheckoutResponse> { checkout3, checkout2, checkout1 };

            // Act
            var actualAfterSortList = await _checkoutService.Sort(sortBy, beforeSort);

            // Assert
            Assert.AreEqual(expectedAfterSortList, actualAfterSortList);
        }

        [Test]
        public async Task SortByDateAsc_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            string sortBy = SortOption.DateAsc.GetDescription();
            var expectedAfterSortList = new List<CheckoutResponse>();

            // Act
            var actualAfterSortList = await _checkoutService.Sort(sortBy, expectedAfterSortList);

            // Assert
            Assert.AreEqual(expectedAfterSortList, actualAfterSortList);
        }

        [Test]
        public async Task SortByDateDesc_DiffrentYears_SortedCheckoutResponses()
        {
            // Arrange
            DateTime dateCheckout1 = new DateTime(2010, 2, 9, 8, 30, 2);
            DateTime dateCheckout2 = new DateTime(2020, 2, 9, 8, 30, 2);
            DateTime dateCheckout3 = new DateTime(2015, 2, 9, 8, 30, 2);
            string sortBy = SortOption.DateDesc.GetDescription();

            var checkout1 = new CheckoutResponse
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = dateCheckout1,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = true,
                UserEmail = "test",
                CheckoutId = 1
            };

            var checkout2 = new CheckoutResponse
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = dateCheckout2,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = false,
                UserEmail = "test2",
                CheckoutId = 2
            };

            var checkout3 = new CheckoutResponse
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = dateCheckout3,
                Description = "test",
                GroupId = 3,
                UserId = "userId",
                IsSplitted = true,
                UserEmail = "test3",
                CheckoutId = 3
            };

            var beforeSort = new List<CheckoutResponse> { checkout1, checkout2, checkout3 };
            var expectedAfterSortList = new List<CheckoutResponse> { checkout2, checkout3, checkout1 };

            // Act
            var actualAfterSortList = await _checkoutService.Sort(sortBy, beforeSort);

            // Assert
            Assert.AreEqual(expectedAfterSortList, actualAfterSortList);
        }

        [Test]
        public async Task SortByDateDesc_SameYearsDiffrentMonths_SortedCheckoutResponses()
        {
            // Arrange
            DateTime dateCheckout1 = new DateTime(2010, 11, 9, 8, 30, 2);
            DateTime dateCheckout2 = new DateTime(2010, 10, 9, 8, 30, 2);
            DateTime dateCheckout3 = new DateTime(2010, 5, 9, 8, 30, 2);
            string sortBy = SortOption.DateDesc.GetDescription();

            var checkout1 = new CheckoutResponse
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = dateCheckout1,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = true,
                UserEmail = "test",
                CheckoutId = 1
            };

            var checkout2 = new CheckoutResponse
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = dateCheckout2,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = false,
                UserEmail = "test2",
                CheckoutId = 2
            };

            var checkout3 = new CheckoutResponse
            {
                Currency = "EUR",
                Price = 100,
                CreatedAt = dateCheckout3,
                Description = "test",
                GroupId = 3,
                UserId = "userId",
                IsSplitted = true,
                UserEmail = "test3",
                CheckoutId = 3
            };

            var beforeSort = new List<CheckoutResponse> { checkout1, checkout2, checkout3 };
            var expectedAfterSortList = new List<CheckoutResponse> { checkout1, checkout2, checkout3 };

            // Act
            var actualAfterSortList = await _checkoutService.Sort(sortBy, beforeSort);

            // Assert
            Assert.AreEqual(expectedAfterSortList, actualAfterSortList);
        }

        [Test]
        public async Task SortByDateDesc_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            string sortBy = SortOption.DateDesc.GetDescription();
            var expectedAfterSortList = new List<CheckoutResponse>();

            // Act
            var actualAfterSortList = await _checkoutService.Sort(sortBy, expectedAfterSortList);

            // Assert
            Assert.AreEqual(expectedAfterSortList, actualAfterSortList);
        }

        [TestCase(
            100, "PLN", false, "user1",
            200, "PLN", false, "user1",
            300, "PLN", false, "user1",
            "PLN", 2, "user1", 600)]
        [TestCase(
            100, "PLN", false, "user1",
            200, "PLN", false, "user1",
            300, "PLN", false, "user1",
            "PLN", 10, "user1", 600)]
        [TestCase(
            100, "PLN", false, "user1",
            200, "PLN", false, "user1",
            300, "PLN", false, "user1",
            "PLN", 2, "user2", -600)]
        [TestCase(
            100, "PLN", false, "user1",
            200, "PLN", false, "user1",
            300, "PLN", false, "user1",
            "PLN", 3, "user2", -300)]
        [TestCase(
            100, "PLN", false, "user1",
            200, "PLN", false, "user3",
            300, "PLN", false, "user3",
            "PLN", 3, "user2", -300)]

        [TestCase(
            100, "PLN", false, "user1",
            200, "PLN", false, "user1",
            300, "PLN", false, "user1",
            "GBP", 2, "user1", 600 / 5)]
        [TestCase(
            100, "PLN", false, "user1",
            200, "PLN", false, "user1",
            300, "PLN", false, "user1",
            "GBP", 10, "user1", 600 / 5)]
        [TestCase(
            100, "PLN", false, "user1",
            200, "PLN", false, "user1",
            300, "PLN", false, "user1",
            "GBP", 2, "user2", -600 / 5)]
        [TestCase(
            100, "PLN", false, "user1",
            200, "PLN", false, "user1",
            300, "PLN", false, "user1",
            "GBP", 3, "user2", -300 / 5)]
        [TestCase(
            100, "PLN", false, "user1",
            200, "PLN", false, "user3",
            300, "PLN", false, "user3",
            "GBP", 3, "user2", -300 / 5)]


        [TestCase(
            100, "GBP", false, "user1",
            200, "GBP", false, "user1",
            300, "GBP", false, "user1",
            "PLN", 2, "user1", 600 * 5)]
        [TestCase(
            100, "GBP", false, "user1",
            200, "GBP", false, "user1",
            300, "GBP", false, "user1",
            "PLN", 10, "user1", 600 * 5)]
        [TestCase(
            100, "GBP", false, "user1",
            200, "GBP", false, "user1",
            300, "GBP", false, "user1",
            "PLN", 2, "user2", -600 * 5)]
        [TestCase(
            100, "GBP", false, "user1",
            200, "GBP", false, "user1",
            300, "GBP", false, "user1",
            "PLN", 3, "user2", -300 * 5)]
        [TestCase(
            100, "GBP", false, "user1",
            200, "GBP", false, "user3",
            300, "GBP", false, "user3",
            "PLN", 3, "user2", -300 * 5)]

        [TestCase(
            100, "PLN", true, "user1",
            200, "PLN", true, "user1",
            300, "PLN", true, "user1",
            "PLN", 2, "user1", 600 / 2)]
        [TestCase(
            100, "PLN", true, "user1",
            200, "PLN", true, "user1",
            300, "PLN", true, "user1",
            "PLN", 3, "user1", 600 / 3)]
        [TestCase(
            100, "PLN", true, "user1",
            200, "PLN", true, "user1",
            300, "PLN", true, "user1",
            "PLN", 2, "user2", -600 / 2)]
        [TestCase(
            100, "PLN", true, "user1",
            200, "PLN", true, "user1",
            300, "PLN", true, "user1",
            "PLN", 3, "user2", -600 / 3)]
        [TestCase(
            100, "PLN", true, "user1",
            200, "PLN", true, "user3",
            300, "PLN", true, "user3",
            "PLN", 3, "user2", -600 / 3)]

        [TestCase(
            100, "PLN", false, "user1",
            200, "PLN", false, "user1",
            300, "PLN", false, "user2",
            "PLN", 2, "user2", 300 - 300)]
        [TestCase(
            100, "PLN", false, "user1",
            200, "PLN", false, "user1",
            400, "PLN", true, "user2",
            "PLN", 2, "user2", (400 / 2) - (100+200))]
        [TestCase(
            100, "PLN", true, "user1",
            200, "PLN", false, "user1",
            400, "PLN", true, "user2",
            "PLN", 2, "user2", (400 / 2) - ((100 / 2) + 200))]
        [TestCase(
            100, "GBP", true, "user1",
            200, "PLN", false, "user1",
            400, "GBP", true, "user2",
            "PLN", 2, "user2", (((400 / 2) * 5) - (((100 / 2) * 5) + 200)))]
        [TestCase(
            30, "GBP", true, "user1",
            200, "PLN", false, "user1",
            300, "GBP", true, "user2",
            "PLN", 3, "user2", (((300 / 3) * 5) - (((30 / 3) * 5 + (200 / (3 - 1))))))] 

        public async Task ComputeTotalBalanceTest
            (
            decimal price1, string currency1, bool isSplitted1, string email1,
            decimal price2, string currency2, bool isSplitted2, string email2,
            decimal price3, string currency3, bool isSplitted3, string email3,
            string baseCurrency, int numOfMembers, string forUserEmail, decimal expectedTotal
            )
        {
            // Arrange
            var checkout1 = new CheckoutResponse
            {
                Currency = currency1,
                Price = price1,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = isSplitted1,
                UserEmail = email1,
                CheckoutId = 1
            };

            var checkout2 = new CheckoutResponse
            {
                Currency = currency2,
                Price = price2,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = isSplitted2,
                UserEmail = email2,
                CheckoutId = 2
            };

            var checkout3 = new CheckoutResponse
            {
                Currency = currency3,
                Price = price3,
                CreatedAt = DateTime.UtcNow,
                Description = "test",
                GroupId = 1,
                UserId = "userId",
                IsSplitted = isSplitted3,
                UserEmail = email3,
                CheckoutId = 3
            };
            var checkouts = new List<CheckoutResponse> { checkout1, checkout2, checkout3 };

            // Act
            var actualTotal = await _checkoutService.ComputeTotalBalance(forUserEmail, baseCurrency, numOfMembers, checkouts);

            // Assert
            Assert.AreEqual(expectedTotal, actualTotal);
        }   
    }
}