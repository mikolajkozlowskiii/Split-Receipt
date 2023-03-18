using Split_Receipt.Data;
using Split_Receipt.Models;
using Split_Receipt.Payload;
using System;

namespace Split_Receipt.Services.Interfaces
{
    public interface IGroupService
    {
        Group Get(int id);
        List<Group> GetAll();
        int Save(Group group);

        Task<List<UserGroupResponse>> GetAllUserGroups();
        void Save(UserGroupRequest request, string emailOfLoggedUser);
        int Save(List<User_Group> userGroups);
    }
}
