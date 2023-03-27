using Microsoft.AspNetCore.Identity;
using Split_Receipt.Areas.Identity.Data;
using Split_Receipt.Data;
using Split_Receipt.Models;
using Split_Receipt.Payload;
using Split_Receipt.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

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

        public Group FindById(int id)
        {
            return _appContext.Groups.FirstOrDefault(g => g.Id == id);
        }

        public List<Group> FindAll() 
        { 
            return _appContext.Groups.ToList();
        }

        public int Save(Group group) 
        {
            _appContext.Groups.Add(group);

            return _appContext.SaveChanges();
        }

        public async Task<List<UserGroupResponse>> FindAllUserGroupsByUserId(string userId)
        {
            var userGroupsId = _appContext.User_Groups
                                            .Where(x => x.UserId == userId)
                                            .Select(x => x.GroupId)
                                            .ToList();

            List<User_Group> allUserGroups = _appContext.User_Groups
                .Where(x => userGroupsId
                .Contains(x.GroupId))
                .ToList();

            List<UserGroupResponse> userGroupResponses = await map(allUserGroups);
            return userGroupResponses;
        }

        public async Task<List<UserGroupResponse>> FindAllUserGroups()
        {
            List<User_Group> userGroups = _appContext.User_Groups.ToList();
            List<UserGroupResponse> userGroupResponses = await map(userGroups);
            return userGroupResponses;
        }

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

        public int Save(List<User_Group> userGroups)
        {
            _appContext.User_Groups.AddRange(userGroups);
            return _appContext.SaveChanges();
        }

        public async Task<Boolean> Save(UserGroupRequest request)
        {
            List<String> emails = await GetUniqueExistingUsers(request.Emails);
            ValidateEmails(emails);

            int groupId = CreateGroup(request).Id;

            List<User_Group> userGroups = new List<User_Group>();
            foreach (var email in emails)
            {
                 var user = await _userManager.FindByEmailAsync(email);
                 userGroups.Add(new User_Group(groupId, user.Id));
            }

            Save(userGroups);
            return true;
        }
        private async Task<List<string>> GetUniqueExistingUsers(List<String> emails)
        {
            HashSet<string> existingEmails = new HashSet<string>();

            if (emails != null)
            {
                foreach (var email in emails)
                {
                    if(email != null)
                    {
                        var user = await _userManager.FindByEmailAsync(email);
                        if (user != null)
                        {
                            existingEmails.Add(email);
                        }
                    }           
                }
            }

            return existingEmails.ToList();
        }

        private void ValidateEmails(List<string> emails)
        {
            if (emails == null || emails.Count < 2)
            {
                throw new ValidationException("There need to be 2 emails in user_group at least.");
            }
        }

        private void ValidateGroupName(String groupName)
        {
            if (groupName == null || groupName.Length<3 || groupName.Length > 20)
            {
                throw new ValidationException("Group name should have lenght beetween 3 and 20.");
            }
        }

        private Group CreateGroup(UserGroupRequest request)
        {
            String groupName = request.GroupName;
            ValidateGroupName(groupName);
            Group group = new Group();
            group.Name = groupName;
            Save(group);
            return group;
        }

        public bool CheckIsUserInGroup(string userId, int groupId)
        {
            return _appContext.User_Groups.Any(x => x.GroupId == groupId && x.UserId == userId);
        }

        public async Task<List<String>> GetAllMembersIDs(int groupId)
        {
            return _appContext.User_Groups
                                           .Where(x => x.GroupId == groupId)
                                           .Select(x => x.UserId)
                                           .ToList();
        }
        public async Task<List<String>> GetAllMembersEmails(int groupId)
        {
            var membersId = await GetAllMembersIDs(groupId);
            List<String> emails = new List<string>();
            foreach (var id in membersId)
            {
                var user = await _userManager.FindByIdAsync(id);
                emails.Add(user.Email);
            }
            return emails;
        }
    }
}
