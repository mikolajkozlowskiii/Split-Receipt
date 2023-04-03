using Microsoft.AspNetCore.Identity;
using Split_Receipt.Areas.Identity.Data;
using Split_Receipt.Models;
using Split_Receipt.Payload;

namespace Split_Receipt.Services.Mappers
{
    /// <summary>
    /// The <c>CheckoutMapper</c> class provides methods for mapping objects of the
    /// Checkout class to objects of the CheckoutResponse class. This class uses the
    /// <c>UserManager<ApplicationUser></c> class from the <c>Microsoft.AspNetCore.Identity</c>
    /// namespace to retrieve user information, such as email. The <c>map</c> method takes a
    /// Checkout object and maps it to a CheckoutResponse object. The <c>map</c> method also
    /// takes a list of Checkout objects and maps them to a list of CheckoutResponse objects.
    /// This class helps with the serialization of Checkout objects to CheckoutResponse objects,
    /// which is useful in returning data to the client side.
    /// </summary>
    public class CheckoutMapper
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutMapper(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public  CheckoutResponse map(Checkout? checkout, String email)
        {
            return new CheckoutResponse {
                IsSplitted = checkout.IsSplitted,
                Currency = checkout.Currency,
                Price = checkout.Price,
                Description = checkout.Description,
                UserEmail = email,
                GroupId = checkout.GroupId,
                UserId = checkout.UserId,
                CheckoutId = checkout.Id,
                CreatedAt = checkout.CreatedAt
            };
        }

        public async Task<List<CheckoutResponse>> map(List<Checkout> checkouts)
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
    }
}
