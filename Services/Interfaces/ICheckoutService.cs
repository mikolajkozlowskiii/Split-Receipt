using Split_Receipt.Payload;

namespace Split_Receipt.Services.Interfaces
{
    /// <summary>
    /// Interface <t>ICheckoutService</t> that contains methods for operation on
    /// <c>CheckoutResponse</c>, <c>CheckoutRequest</c> and <c>CheckoutSummary</c>.
    /// </summary>
    public interface ICheckoutService
    {
        Task<int> Save(CheckoutRequest requestCheckout, string userId, int groupId);
        int Update(CheckoutRequest checkoutRequest, int checkoutId);
        int Delete(int id);
        bool CheckIsUserInCheckout(string UserId, int checkoutId);
        Task<CheckoutResponse> FindById(int id);
        Task<List<CheckoutResponse>> FindAll();
        Task<List<CheckoutResponse>> FindlAllByGroupId(int groupId, string sortBy);
        Task<List<CheckoutResponse>> FindAllByUserID(string userId);
        Task<CheckoutSummary> CreateCheckoutSummary(string userEmail, string currencyBase, int groupId, string sortBy);     
        Task<decimal> ComputeTotalBalance(string userEmail, string currencyBase, int numOfMemebers, List<CheckoutResponse> checkouts);
        Task<List<CheckoutResponse>> Sort(string sortBy, List<CheckoutResponse> responseList);
    }
}