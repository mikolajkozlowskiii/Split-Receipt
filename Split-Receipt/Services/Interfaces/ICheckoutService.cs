using Split_Receipt.Payload;

/// <summary>
/// In the <c>Split_Receipt.Services.Interfaces</c> namespace, there are several interfaces defined for
/// the various services in the Split_Receipt application. These interfaces define methods for operations on various models,
/// such as <c>Checkout</c>, <c>Group</c>, and <c>Currency</c>. These interfaces define methods for operations on various objects and models
/// like <c>CheckoutResponse</c>, <c>CheckoutRequest</c>, <c>CheckoutSummary</c>, <c>CurrencyResponse</c>,
/// <c>UserGroupResponse</c>, <c>UserGroupRequest</c>, <c>Group</c>, and <c>User_Group</c>. The <c>ICurrencyService</c> interface
/// is responsible for connecting to an external currency API that provides current exchange rates. This interface includes methods
/// for getting the latest currency data and for getting the exchange rate between two currencies.
/// </summary>
namespace Split_Receipt.Services.Interfaces
{
    /// <summary>
    /// The <c>ICheckoutService</c> interface contains methods for performing various
    /// operations related to checkouts such as saving, updating, and deleting checkouts.
    /// Additionally, it includes methods for retrieving checkouts by ID, user ID, or group ID,
    /// as well as methods for creating a checkout summary and computing the total balance of a group's checkouts.
    /// The methods take in objects of type <c>CheckoutResponse</c>, <c>CheckoutRequest</c>, and <c>CheckoutSummary</c>
    /// to perform these operations. The interface is intended to be implemented by classes that
    /// provide concrete implementations of these methods.
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