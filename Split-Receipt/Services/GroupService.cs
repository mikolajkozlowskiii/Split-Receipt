using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Split_Receipt.Areas.Identity.Data;
using Split_Receipt.Data;
using Split_Receipt.Models;
using Split_Receipt.Payload;
using Split_Receipt.Services.Interfaces;
using Split_Receipt.Services.Mappers;
using System.ComponentModel.DataAnnotations;

namespace Split_Receipt.Services
{
    /// <summary>
    /// The <c>GroupService</c> class contains methods related to groups such as finding groups by their id, finding all groups,
    /// saving a group in the database, finding all user groups, finding all user groups belonging to a specific user,
    /// finding all user groups belonging to a specific user and mapping them to user group response objects,
    /// saving a list of user groups in the database.
    /// </summary>
    public class GroupService : IGroupService
    {
        private readonly AuthDbContext _appContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GroupMapper _groupMapper;

        public GroupService(AuthDbContext appContext, UserManager<ApplicationUser> userManager, GroupMapper groupMapper)
        {
            _appContext = appContext;
            _userManager = userManager;
            _groupMapper = groupMapper;
        }

        /// <summary>
        /// This method is used for find Group's object instance by group's id.
        /// </summary>
        /// <param name="id"></param> is an id of group.
        /// <returns>Instance of Group's object.</returns>
        /// <exception cref="InvalidOperationException">If there is no such a group's id in DB,
        /// then it throws InvalidOperationException with a proper message.</exception>
        public Group FindById(int id)
        {
            var group = _appContext.Groups.FirstOrDefault(g => g.Id == id);
            if (group == null)
            {
                throw new InvalidOperationException($"Group with id {id} was not found.");
            }
            return group;
        }

        /// <summary>
        /// This method is used for find all Group's object instances in DB.
        /// </summary>
        /// <returns>List of Group's object instances saved in DB.</returns>
        public List<Group> FindAll() 
        {
            return _appContext.Groups.ToList();
        }

        /// <summary>
        /// This method is used for save Group's object instance in DB.
        /// </summary>
        /// <param name="group"></param> is a Group's object to be saved in DB
        /// <returns>Positive integer if save operation ended successfully, otherwise negative number.</returns>
        public int Save(Group group) 
        {
            var context = new ValidationContext(group, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(group, context, validationResults, true);

            if (isValid)
            {
                _appContext.Groups.Add(group);
            }
            return _appContext.SaveChanges();
        }

        /// <summary>
        /// This method is used for find all User_Group object instances in DB.
        /// </summary>
        /// <returns>List of User_Group's object instances saved in DB.</returns>
        public async Task<List<User_Group>> FindAllUserGroups()
        {
            return _appContext.User_Groups.ToList();
        }

        /// <summary>
        /// This method is used to find all User_Group's object instance saved in DB.
        /// </summary>
        /// <param name="userId"></param> is a user's id used to find User_Groups which contains this id.
        /// <returns>>List of User_Group's object instances</returns>
        public async Task<List<User_Group>> FindAllUserGroupsByUserId(string userId)
        {
            var userGroupsId = _appContext.User_Groups
                                            .Where(x => x.UserId == userId)
                                            .Select(x => x.GroupId)
                                            .ToList();

            return _appContext.User_Groups
                .Where(x => userGroupsId
                .Contains(x.GroupId))
                .ToList();
        }

        /// <summary>
        /// This method is used for find all User_Group's object instances saved in DB where belogns specific user.
        /// </summary>
        /// <param name="userId"></param> is a user's id, due that it is possible to recognize, where specific User, belongs
        /// to which User_Group table.
        /// <returns>List of User_Group's object instances mapped on UserGroupResponse's object instances.</returns>
        public async Task<List<UserGroupResponse>> FindAllUserGroupsResponseByUserId(string userId)
        {
            List<UserGroupResponse> userGroupResponses = await _groupMapper.map(await FindAllUserGroupsByUserId(userId));
            return userGroupResponses;
        }

        /// <summary>
        /// This method is used for find all User_Group's object instance saved in DB.
        /// </summary>
        /// <returns>List of User_Group's object instances mapped on UserGroupResponse's object instances.</returns>
        public async Task<List<UserGroupResponse>> FindAllUserGroupsResponse()
        {
            List<User_Group> userGroups = await FindAllUserGroups();
            List<UserGroupResponse> userGroupResponses = await _groupMapper.map(userGroups);
            return userGroupResponses;
        }

        /// <summary>
        /// This method is used for save list of User_Group's object instances in DB.
        /// </summary>
        /// <param name="userGroups"></param> is a body of User_Group's object instance.
        /// <returns>Positive integer if save operation ended successfully. Otherwise negative number.</returns>
        public int Save(List<User_Group> userGroups)
        {
            foreach(var userGroup in userGroups)
            {
                var context = new ValidationContext(userGroup, serviceProvider: null, items: null);
                var validationResults = new List<ValidationResult>();

                bool isValid = Validator.TryValidateObject(userGroup, context, validationResults, true);
                if (!isValid)
                {
                    return -1;
                }
            }
            _appContext.User_Groups.AddRange(userGroups);
            return _appContext.SaveChanges();
        }

        /// <summary>
        /// This method is used for take UserGroupRequest's object instance body. 
        /// Then it call Save method and save object in DB. Due to private method 
        /// GetUniqueExistingUsers(request.Emails) this method is procted from save empty email,
        /// save email not existed in user's table in DB or save more than once member with the same email.
        /// </summary>
        /// <param name="request"></param> is a body of UserGroupRequest's object instance.
        /// <returns>True if save operation ended successfully. Otherwise false.</returns>
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

            if (Save(userGroups) > 0)
            {
                return true;
            }
            return false;
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

        /// <summary>
        /// This method is used for check if specific user belongs to specific group.
        /// </summary>
        /// <param name="userId"></param> is an id of user.
        /// <param name="groupId"></param> is an id of group.
        /// <returns>True if user belongs to specific group. Otherwise false.</returns>
        public async Task<Boolean> CheckIsUserInGroup(string userId, int groupId)
        {
            if(await _userManager.FindByIdAsync(userId) == null)
            {
                throw new InvalidOperationException("User id: " + userId + " is not in DB.");
            }
            return _appContext.User_Groups.Any(x => x.GroupId == groupId && x.UserId == userId);
        }

        /// <summary>
        /// This method finds all user's ids which belogns to specific group.
        /// </summary>
        /// <param name="groupId"></param> is an id of group.
        /// <returns>List of users' ids which belongs to specific group.</returns>
        public async Task<List<String>> GetAllMembersIDs(int groupId)
        {
            return _appContext.User_Groups
                                           .Where(x => x.GroupId == groupId)
                                           .Select(x => x.UserId)
                                           .ToList();
        }

        /// <summary>
        /// This method finds all user's email which belogns to specific group.
        /// </summary>
        /// <param name="groupId"></param> is an id of group.
        /// <returns>List of users' emails which belongs to specific group.</returns>
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
