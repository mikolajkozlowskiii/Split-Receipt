﻿using Microsoft.AspNetCore.Identity;
using Split_Receipt.Areas.Identity.Data;
using Split_Receipt.Data;
using Split_Receipt.Models;
using Split_Receipt.Payload;

/// <summary>
/// The namespace Split_Receipt.Services.Mappers contains mapper classes that help to map requests to models
/// that are saved in the database, and models from the database to responses. 
/// </summary>
namespace Split_Receipt.Services.Mappers
{
    /// <summary>
    /// The <c>GroupMapper</c> class contains methods for mapping Group's object instances.
    /// It takes <c>User_Group</c> objects and maps them to a list of <c>UserGroupResponse</c>
    /// objects containing the group ID, group name, and user email. This class uses the <c>AuthDbContext</c>
    /// and <c>UserManager<ApplicationUser></c> for data access and user management, respectively.
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
