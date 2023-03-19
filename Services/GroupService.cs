using Microsoft.AspNetCore.Identity;
using Split_Receipt.Areas.Identity.Data;
using Split_Receipt.Data;
using Split_Receipt.Models;
using Split_Receipt.Payload;
using Split_Receipt.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Split_Receipt.Services
{
    public class GroupService : IGroupService
    {
        private readonly AuthDbContext _appContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public GroupService(AuthDbContext appContext, UserManager<ApplicationUser> userManager)
        {
            _appContext = appContext;
            _userManager = userManager;
        }

        public Group Get(int id)
        {
            return _appContext.Groups.FirstOrDefault(g => g.Id == id);
        }

        public List<Group> GetAll() 
        { 
            return _appContext.Groups.ToList();
        }

        public int Save(Group group) 
        {
            _appContext.Groups.Add(group);
            if (_appContext.SaveChanges() > 0)
            {
                Console.WriteLine("Succes");
            };
            return _appContext.SaveChanges();
        }

        public async Task<List<UserGroupResponse>> FindAllUserGroupsByUserId(string userId)
        {
            var userGroupsId = _appContext.User_Groups
                                            .Where(x => x.UserId == userId)
                                            .Select(x => x.GroupId)
                                            .ToList();

            List<User_Group> allUserGroups = _appContext.User_Groups.Where(x => userGroupsId.Contains(x.GroupId)).ToList(); // wszystkie serGroup

            List<UserGroupResponse> userGroupResponses = new List<UserGroupResponse>();
            foreach (User_Group userGroup in allUserGroups)
            {
                Group group = _appContext.Groups.Find(userGroup.GroupId);
                String groupName = group.Name;
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

        public async Task<List<UserGroupResponse>> GetAllUserGroups()
        {
            List<User_Group> userGroups = _appContext.User_Groups.ToList();
            List<UserGroupResponse> userGroupResponses = new List<UserGroupResponse>();
            foreach(User_Group userGroup in userGroups)
            {
                Group group = _appContext.Groups.Find(userGroup.GroupId);
                String groupName = group.Name;
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

        public int Save(List<User_Group> userGroups)
        {
            _appContext.User_Groups.AddRange(userGroups);
            _appContext.SaveChanges();
            return 1;
        }

        public async Task<Boolean> Save(UserGroupRequest request, string emailOfLoggedUser)
        {
            List<User_Group> userGroups = new List<User_Group>();
            HashSet<String> emails = new HashSet<string>();
        
            int groupId = createGroup(request).Id;

            // adding current logged in user
            var user = await _userManager.FindByEmailAsync(emailOfLoggedUser);
            if (user != null)
            {
                emails.Add(user.Email);
            }

            // adding emails from request
            foreach (var email in request.Emails)
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    bool isUserInGroup = _appContext.User_Groups.Any(x => x.Id == groupId && x.UserId == user.Id);
                    if (!isUserInGroup)
                    {
                        emails.Add(email);
                    }
                }
            }

            if(emails.Count < 2) // group without more than 1 memeber = useless
            {
                return false;
            }

            foreach (var email in emails)
            {
                user = await _userManager.FindByEmailAsync(email);
                userGroups.Add(new User_Group(groupId, user.Id));
            }

            Save(userGroups);
            return true;
        }

        private Group createGroup(UserGroupRequest request)
        {
            String groupName = request.GroupName;
            Group group = new Group();
            group.Name = groupName;
            Save(group);
            return group;
        } 
    }
}
