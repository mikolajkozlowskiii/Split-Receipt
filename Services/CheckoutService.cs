using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Split_Receipt.Areas.Identity.Data;
using Split_Receipt.Data;
using Split_Receipt.Models;
using Split_Receipt.Payload;
using Split_Receipt.Services.Interfaces;
using System.Text.RegularExpressions;

namespace Split_Receipt.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly AuthDbContext _appContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGroupService _groupService;
        private readonly ICurrencyService _currencyService;

        public CheckoutService(AuthDbContext appContext, UserManager<ApplicationUser> userManager,
            IGroupService groupService, ICurrencyService currencyService)
        {
            _appContext = appContext;
            _userManager = userManager;
            _groupService = groupService;
            _currencyService = currencyService;
        }

        public int delete(int id)
        {
            var checkout = _appContext.Checkouts.Find(id);
            _appContext.Checkouts.Remove(checkout);
            _appContext.SaveChanges();
            return id;
        }

        public async Task<CheckoutResponse> get(int id)
        {
            var checkout = _appContext.Checkouts.FirstOrDefault(x => x.Id == id);
            var user = await _userManager.FindByIdAsync(checkout.UserId);

            CheckoutResponse response = map(checkout, user.Email);

            return response;
        }

        private static CheckoutResponse map(Checkout? checkout, String email)
        {
            CheckoutResponse response = new CheckoutResponse();
            response.IsSplitted = checkout.IsSplitted;
            response.Currency = checkout.Currency;
            response.Price = checkout.Price;
            response.Description = checkout.Description;
            response.UserEmail = email;
            response.GroupId = checkout.GroupId;
            response.UserId = checkout.UserId;
            response.CheckoutId = checkout.Id;
            response.CreatedAt = checkout.CreatedAt;

            return response;
        }

        public async Task<List<CheckoutResponse>> getAll()
        {
            var checkouts = _appContext.Checkouts.ToList();
            return await map(checkouts);
        }

        private async Task<List<CheckoutResponse>> map(List<Checkout> checkouts)
        {
            List<CheckoutResponse> responses = new List<CheckoutResponse>();
            foreach (var checkout in checkouts)
            {
                var user = await _userManager.FindByIdAsync(checkout.UserId);

                CheckoutResponse response = new CheckoutResponse();
                response.IsSplitted = checkout.IsSplitted;
                response.Currency = checkout.Currency;
                response.Price = checkout.Price;
                response.Description = checkout.Description;
                response.UserEmail = user.Email;
                response.CheckoutId = checkout.Id;
                response.CreatedAt = checkout.CreatedAt;

                responses.Add(response);
            }

            return responses;
        }

        public async Task<List<CheckoutResponse>> getAllByGroupID(int groupId)
        {
            var checkouts = _appContext.Checkouts
                .Where(x => x.GroupId == groupId)
                .ToList();
            return await map(checkouts);

        }

        public async Task<List<CheckoutResponse>> getAllByUserID(string userId)
        {
            var checkouts = _appContext.Checkouts
                .Where(x => x.UserId == userId)
                .ToList();
            return await map(checkouts);
        }


        public int save(CheckoutRequest requestCheckout, string userId, int groupId)
        {
            // check if group exists chociaz w sumie moze to nizej wystarczy
            // check if user is in group??
            Checkout checkout = new Checkout();
           
           checkout.Currency = requestCheckout.Currency.ToUpper();
           checkout.Price = requestCheckout.Price;
           checkout.Description = requestCheckout.Description;
           checkout.IsSplitted = requestCheckout.IsSplitted;
           checkout.UserId = userId;
           checkout.GroupId = groupId;
            checkout.CreatedAt = DateTime.Now;
            _appContext.Checkouts.Add(checkout);

            return _appContext.SaveChanges();
        }

        public int update(CheckoutRequest checkoutRequest, int checkoutId)
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

        public async Task<CheckoutSummary> getCheckoutSummary(string userEmail, string currencyBase, int groupId)
        {
            List<string> members = await _groupService.GetAllMembersEmails(groupId);
            List<CheckoutResponse> allCheckouts = await getAllByGroupID(groupId);
            var groupName = _groupService.FindById(groupId).Name;
            int numOfMemebers = members.Count();
            decimal total = await ComputeTotalBalance(userEmail, currencyBase, numOfMemebers, allCheckouts);

            CheckoutSummary checkoutSummary = new CheckoutSummary();
            checkoutSummary.Email = userEmail;
            checkoutSummary.GroupName = groupName;
            checkoutSummary.Checkouts = allCheckouts;
            checkoutSummary.Total = total;
            checkoutSummary.Currency = currencyBase.ToUpper();
            checkoutSummary.Members = members;
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
