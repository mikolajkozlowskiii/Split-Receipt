using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Split_Receipt.Areas.Identity.Data;
using Split_Receipt.Data;
using Split_Receipt.Models;
using Split_Receipt.Payload;
using Split_Receipt.Services.Interfaces;
using Split_Receipt.Services.Mappers;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Split_Receipt.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly AuthDbContext _appContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGroupService _groupService;
        private readonly ICurrencyService _currencyService;
        private readonly CheckoutMapper _checkoutMapper;

        public CheckoutService(AuthDbContext appContext, UserManager<ApplicationUser> userManager,
            IGroupService groupService, ICurrencyService currencyService, CheckoutMapper checkoutMapper)
        {
            _appContext = appContext;
            _userManager = userManager;
            _groupService = groupService;
            _currencyService = currencyService;
            _checkoutMapper = checkoutMapper;
        }

        public int Delete(int id)
        {
            var checkout = _appContext.Checkouts.Find(id);
            _appContext.Checkouts.Remove(checkout);
            _appContext.SaveChanges();
            return id;
        }

        public async Task<CheckoutResponse> FindById(int id)
        {
            var checkout = _appContext.Checkouts.FirstOrDefault(x => x.Id == id);
            var user = await _userManager.FindByIdAsync(checkout.UserId);
    
            CheckoutResponse response =  _checkoutMapper.map(checkout, user.Email);

            return response;
        }


        public async Task<List<CheckoutResponse>> FindAll()
        {
            var checkouts = _appContext.Checkouts.ToList();
            return await _checkoutMapper.map(checkouts);
        }

        public async Task<List<CheckoutResponse>> FindlAllByGroupId(int groupId, string sortBy)
        {
            var checkouts = _appContext.Checkouts
                .Where(x => x.GroupId == groupId)
                .ToList();
            List<CheckoutResponse> responseList = await _checkoutMapper.map(checkouts);
            return await Sort(sortBy, responseList);
        }

        private async Task<List<CheckoutResponse>> Sort(string sortBy, List<CheckoutResponse> responseList)
        {
            var baseCurrency = "PLN";
            Dictionary<CheckoutResponse, decimal> checkoutDict;
            switch (sortBy)
            {
                case "Price ASC":
                    checkoutDict = await GetCheckoutEquivalentPriceDict(responseList, baseCurrency);
                    var sortedDict = from entry in checkoutDict orderby entry.Value ascending select entry;
                    return sortedDict.Select(entry => entry.Key).ToList();

                case "Price DESC":
                    checkoutDict = await GetCheckoutEquivalentPriceDict(responseList, baseCurrency);
                    var sortedDict2 = from entry in checkoutDict orderby entry.Value descending select entry;
                    return sortedDict2.Select(entry => entry.Key).ToList();

                case "Date ASC":
                    responseList.Sort((x, y) => x.CreatedAt.CompareTo(y.CreatedAt));
                    break;

                case "Date DESC":
                    responseList.Sort((x, y) => y.CreatedAt.CompareTo(x.CreatedAt));
                    break;

                default:
                    responseList.Sort((x, y) => x.CreatedAt.CompareTo(y.CreatedAt));
                    break;
            }

            return responseList;
        }

        private async Task<Dictionary<CheckoutResponse, decimal>> GetCheckoutEquivalentPriceDict(List<CheckoutResponse> responseList, string baseCurrency)
        {
            Dictionary<CheckoutResponse, decimal> checkoutDict = new Dictionary<CheckoutResponse, decimal>();
            foreach (var checkoutResponse in responseList)
            {
                decimal priceInSameCurrency = 0;
                if (checkoutResponse.Price != 0)
                {
                    priceInSameCurrency = await (_currencyService.GetRate(baseCurrency, checkoutResponse.Currency)) / (checkoutResponse.Price);
                } 
                checkoutDict.Add(checkoutResponse, priceInSameCurrency);
            }
            return checkoutDict;
        }

        public async Task<List<CheckoutResponse>> FindAllByUserID(string userId)
        {
            var checkouts = _appContext.Checkouts
                .Where(x => x.UserId == userId)
                .ToList();
            return await _checkoutMapper.map(checkouts);
        }


        public int Save(CheckoutRequest requestCheckout, string userId, int groupId)
        {
            Checkout checkout = new Checkout
            {
                Currency = requestCheckout.Currency.ToUpper(),
                Price = requestCheckout.Price,
                Description = requestCheckout.Description,
                IsSplitted = requestCheckout.IsSplitted,
                UserId = userId,
                GroupId = groupId,
                CreatedAt = DateTime.Now
            };         
           _appContext.Checkouts.Add(checkout);

            return _appContext.SaveChanges();
        }

        public int Update(CheckoutRequest checkoutRequest, int checkoutId)
        {
            var checkout = _appContext.Checkouts.Find(checkoutId);
            if(checkout != null)
            {
                checkout.Price = checkoutRequest.Price;
                checkout.Description = checkoutRequest.Description;
                checkout.Currency = checkoutRequest.Currency.ToUpper();
                checkout.IsSplitted= checkoutRequest.IsSplitted;

                _appContext.SaveChanges();

                return checkout.Id;
            }
            return -1;
        }

        public bool CheckIsUserInCheckout(string UserId, int checkoutId)
        {
            bool isUserInCheckout = _appContext.Checkouts.Any(x => x.UserId == UserId && x.Id == checkoutId);
            return isUserInCheckout;
        }

        public async Task<CheckoutSummary> CreateCheckoutSummary(string userEmail, string currencyBase, int groupId, string sortBy)
        {
            List<string> members = await _groupService.GetAllMembersEmails(groupId);
            List<CheckoutResponse> allCheckouts = await FindlAllByGroupId(groupId, sortBy);
            var groupName = _groupService.FindById(groupId).Name;
            int numOfMemebers = members.Count();
            decimal total = await ComputeTotalBalance(userEmail, currencyBase, numOfMemebers, allCheckouts);

            CheckoutSummary checkoutSummary = new CheckoutSummary
            {
                Email = userEmail,
                GroupName = groupName,
                Checkouts = allCheckouts,
                Total = total,
                Currency = currencyBase.ToUpper(),
                Members = members,
            };
        
            return checkoutSummary;
        }

        private async Task<decimal> ComputeTotalBalance(string userEmail, string currencyBase, int numOfMemebers, List<CheckoutResponse> checkouts)
        {
            decimal total = 0;
            foreach (var checkout in checkouts)
            {
                decimal rate = 1;
                if (!currencyBase.Equals(checkout.Currency))
                {
                    rate = await _currencyService.GetRate(currencyBase, checkout.Currency);
                }
                decimal currentPrice = checkout.Price;
                if (checkout.IsSplitted)
                {
                    currentPrice = currentPrice / numOfMemebers;
                }
                else
                {
                    currentPrice = currentPrice / (numOfMemebers - 1);
                }
                if (userEmail.Equals(checkout.UserEmail))
                {
                    total = total + currentPrice / rate;
                }
                else
                {
                    total = total - currentPrice / rate;
                }

            }
            return total;
        }
    }
}
