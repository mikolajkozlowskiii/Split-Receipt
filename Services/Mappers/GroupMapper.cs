using Microsoft.AspNetCore.Identity;
using Split_Receipt.Areas.Identity.Data;
using Split_Receipt.Data;
using Split_Receipt.Models;
using Split_Receipt.Payload;

namespace Split_Receipt.Services.Mappers
{
    public class GroupMapper
    {
        private readonly AuthDbContext _appContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public GroupMapper(AuthDbContext appContext, UserManager<ApplicationUser> userManager)
        {
            _appContext = appContext;
            _userManager = userManager;
        }

        public async Task<List<UserGroupResponse>> map(List<User_Group> userGroups)
        {
            List<UserGroupResponse> userGroupResponses = new List<UserGroupResponse>();
            foreach (User_Group userGroup in userGroups)
            {
                Group group = _appContext.Groups.Find(userGroup.GroupId);
                string groupName = group.Name;
                int groupId = group.Id;
                var user = await _userManager.FindByIdAsync(userGroup.UserId);
                if (user != null)
                {
                    string email = user.Email;
                    userGroupResponses.Add(new UserGroupResponse(groupId, groupName, email));
                }
            }
            return userGroupResponses;
        }
    }
}


/*
  private async Task<List<UserGroupResponse>> map(List<User_Group> userGroups)
        {
            List<UserGroupResponse> userGroupResponses = new List<UserGroupResponse>();
            foreach (User_Group userGroup in userGroups)
            {
                Group group = _appContext.Groups.Find(userGroup.GroupId);
                string groupName = group.Name;
                int groupId = group.Id;
                var user = await _userManager.FindByIdAsync(userGroup.UserId);
                if (user != null)
                {
                    string email = user.Email;
                    userGroupResponses.Add(new UserGroupResponse(groupId, groupName, email));
                }
            }
            return userGroupResponses;
        } 



 
 */