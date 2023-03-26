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
        Task<List<UserGroupResponse>> FindAllUserGroupsByUserId(string userId);
        Task<List<String>> GetAllMembersIDs(int groupId);
        Task<List<String>> GetAllMembersEmails(int groupId);
        Task<Boolean> Save(UserGroupRequest request, string emailOfLoggedUser);
        int Save(List<User_Group> userGroups);
        bool CheckIsUserInGroup(string userId,  int groupId);

    }
}
