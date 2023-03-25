using Split_Receipt.Models;
using Split_Receipt.Payload;

namespace Split_Receipt.Services.Interfaces
{
    public interface ICheckoutService
    {
        Task<CheckoutResponse> get(int id);
        Task<List<CheckoutResponse>> getAll();
        Task<List<CheckoutResponse>> getAllByGroupID(int groupId);
        Task<List<CheckoutResponse>> getAllByUserID(string userId);

        int save(CheckoutRequest checkout, string userId, int groupId);
        int update(CheckoutRequest checkoutRequest, int checkoutId);
        int delete(int id);
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