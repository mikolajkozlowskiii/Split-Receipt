using Split_Receipt.Models;
using Split_Receipt.Payload;

namespace Split_Receipt.Services.Interfaces
{
    public interface ICheckoutService
    {
        Task<CheckoutResponse> FindById(int id);
        Task<List<CheckoutResponse>> FindAll();
        Task<List<CheckoutResponse>> FindlAllByGroupId(int groupId, string sortBy);
        Task<List<CheckoutResponse>> FindAllByUserID(string userId);
        Task<CheckoutSummary> CreateCheckoutSummary(string userEmail, string currencyBase, int groupId, string sortBy);

        int Save(CheckoutRequest checkout, string userId, int groupId);
        int Update(CheckoutRequest checkoutRequest, int checkoutId);
        int Delete(int id);
        bool CheckIsUserInCheckout(string UserId, int checkoutId);
        
    }
}

/*Group Get(int id);
        List<Group> GetAll();
        int Save(Group group);

        Task<List<UserGroupResponse>> GetAllUserGroups();
        Task<List<UserGroupResponse>> FindAllUserGroupsByUserId(string userId);
        Task<Boolean> Save(UserGroupRequest request, string emailOfLoggedUser);
        int Save(List<User_Group> userGroups);*/