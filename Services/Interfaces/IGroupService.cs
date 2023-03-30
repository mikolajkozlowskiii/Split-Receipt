using Split_Receipt.Data;
using Split_Receipt.Models;
using Split_Receipt.Payload;
using System;

namespace Split_Receipt.Services.Interfaces
{
    public interface IGroupService
    {
        Group FindById(int id);
        List<Group> FindAll();
        int Save(Group group);

        Task<List<UserGroupResponse>> FindAllUserGroupsResponse();
        Task<List<User_Group>> FindAllUserGroups();
        Task<List<UserGroupResponse>> FindAllUserGroupsResponseByUserId(string userId);
        Task<List<User_Group>> FindAllUserGroupsByUserId(string userId);
        Task<List<String>> GetAllMembersIDs(int groupId);
        Task<List<String>> GetAllMembersEmails(int groupId);
        Task<Boolean> Save(UserGroupRequest request);
        int Save(List<User_Group> userGroups);
        Task<Boolean> CheckIsUserInGroup(string userId, int groupId);

    }
}
