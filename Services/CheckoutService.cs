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

        public CheckoutService(AuthDbContext appContext, UserManager<ApplicationUser> userManager)
        {
            _appContext = appContext;
            _userManager = userManager;
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
    }
}
