using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using Split_Receipt.Areas.Identity.Data;
using Split_Receipt.Data;
using Split_Receipt.Models;
using Split_Receipt.Payload;
using Split_Receipt.Services;
using Split_Receipt.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_Receipt.Tests.Services
{
    [TestFixture]
    public class GroupServiceTest2
    {
        private AuthDbContext _appContext;
        private UserManager<ApplicationUser> _userManager;
        private IGroupService _groupService;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AuthDbContext>()
                .UseInMemoryDatabase(databaseName: "SplitReceipt")
                .Options;

            _appContext = new AuthDbContext(options);

            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new UserManager<ApplicationUser>(store.Object, null, null, null, null, null, null, null, null);

            _groupService = new GroupService(_appContext, _userManager);
        }

        [TearDown]
        public async Task Cleanup()
        {
            _appContext.Database.EnsureDeleted();

        }

        [Test]
        public void FindByIdGroup_AllParamsOk_FoundGroup()
        {
            // Arrange
            var group = new Group { Name = "Group 1" };
            _appContext.Groups.Add(group);
            _appContext.SaveChanges();

            // Act
            var result = _groupService.FindById(group.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(group.Name, result.Name);
        }


        [Test]
        public void FindByIdGroup_NotExistingId_ThrownException()
        {
            // Arrange
            var group = new Group { Name = "Group 1" };
            _appContext.Groups.Add(group);
            _appContext.SaveChanges();

            // Assert
            Assert.Throws<InvalidOperationException>(() => _groupService.FindById(group.Id + 1));
        }

        [TestCase("group 1", true)]
        [TestCase("group 2", true)]
        [TestCase("1", false)]
        [TestCase("12", true)]
        [TestCase("19charshere12345678", true)]
        [TestCase("20charshere123456789", true)]
        [TestCase("21charshere1234567890", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public void SaveGroupTest(string groupName, bool excpetedSuccess)
        {
            // Arrange
            var group = new Group { Name = groupName };
            bool actualSuccess = false;

            // Act
            if (_groupService.Save(group) > 0)
            {
                actualSuccess = true;
            }

            // Assert
            Assert.AreEqual(excpetedSuccess, actualSuccess);
        }

        [Test]
        public async Task FindAllUserGroupsByUserId_AllParamsOk_AllUserGroupsFound()
        {
            // Arrange
            var userId = "userId123";
            var group1 = new Group { Name = "group 1"};
            var group2 = new Group { Name = "group 2" };
            var group3 = new Group { Name = "group 3" };

            List<Group> groups = new List<Group>() { group1, group2, group3 };
            _appContext.Groups.AddRange(groups); _appContext.SaveChanges();

            var userGroup1 = new User_Group { UserId =  userId, GroupId = group1.Id};
            var userGroup2 = new User_Group { UserId = userId, GroupId = group2.Id };
            var userGroup3 = new User_Group { UserId = userId, GroupId = group3.Id };
            List<User_Group> expectedUserGroups = new List<User_Group>() { userGroup1, userGroup2, userGroup3 };

            _appContext.User_Groups.AddRange(expectedUserGroups); _appContext.SaveChanges();

            // Act
            List<User_Group> actualUserGroups = await _groupService.FindAllUserGroupsByUserId(userId);

            // Assert
            CollectionAssert.AreEquivalent(expectedUserGroups, actualUserGroups);
        }

        [Test]
        public async Task FindAllUserGroupsByUserId_UserIdNotFound_UserGroupsNotFound()
        {
            // Arrange
            var userId = "userId123";
            var diffrentUserId = "userId123456789";
            var group1 = new Group { Name = "group 1" };
            var group2 = new Group { Name = "group 2" };
            var group3 = new Group { Name = "group 3" };

            List<Group> groups = new List<Group>() { group1, group2, group3 };
            _appContext.Groups.AddRange(groups); _appContext.SaveChanges();

            var userGroup1 = new User_Group { UserId = userId, GroupId = group1.Id };
            var userGroup2 = new User_Group { UserId = userId, GroupId = group2.Id };
            var userGroup3 = new User_Group { UserId = userId, GroupId = group3.Id };
            List<User_Group> expectedUserGroups = new List<User_Group>();

            _appContext.User_Groups.AddRange(expectedUserGroups); _appContext.SaveChanges();

            // Act
            List<User_Group> actualUserGroups = await _groupService.FindAllUserGroupsByUserId(diffrentUserId);

            // Assert
            CollectionAssert.AreEquivalent(expectedUserGroups, actualUserGroups);
        }

        [Test]
        public async Task FindAllUserGroupsByUserId_LackOfUserGroupsInDB_UserGroupsNotFound()
        {
            // Arrange
            var userId = "userId123";

            List<User_Group> expectedUserGroups = new List<User_Group>();


            // Act
            List<User_Group> actualUserGroups = await _groupService.FindAllUserGroupsByUserId(userId);

            // Assert
            CollectionAssert.AreEquivalent(expectedUserGroups, actualUserGroups);
        }

        [Test]
        public async Task FindAllUserGroupsBy_AllParamsOk_AllUserGroupsFound()
        {
            // Arrange
            var userId = "userId123";
            var group1 = new Group { Name = "group 1" };
            var group2 = new Group { Name = "group 2" };
            var group3 = new Group { Name = "group 3" };

            List<Group> groups = new List<Group>() { group1, group2, group3 };
            _appContext.Groups.AddRange(groups); _appContext.SaveChanges();

            var userGroup1 = new User_Group { UserId = userId, GroupId = group1.Id };
            var userGroup2 = new User_Group { UserId = userId, GroupId = group2.Id };
            var userGroup3 = new User_Group { UserId = userId, GroupId = group3.Id };
            List<User_Group> expectedUserGroups = new List<User_Group>() { userGroup1, userGroup2, userGroup3 };

            _appContext.User_Groups.AddRange(expectedUserGroups); _appContext.SaveChanges();

            // Act
            List<User_Group> actualUserGroups = await _groupService.FindAllUserGroups();

            // Assert
            CollectionAssert.AreEquivalent(expectedUserGroups, actualUserGroups);
        }

        [Test]
        public async Task FindAllUserGroups_LackOfUserGroupsInDB_UserGroupsNotFound()
        {
            // Arrange
            List<User_Group> expectedUserGroups = new List<User_Group>();


            // Act
            List<User_Group> actualUserGroups = await _groupService.FindAllUserGroups();

            // Assert
            CollectionAssert.AreEquivalent(expectedUserGroups, actualUserGroups);
        }

        [TestCase("group 1", 1, true)]
        [TestCase("group 1", null, true)]
        [TestCase(null, 1, false)]
        [TestCase(null, null, false)]
        public void SaveUserGroupTest(string userId, int groupId, bool expectedSuccess)
        {
            // Arrange
            var userGroup = new User_Group { UserId = userId, GroupId = groupId };
            List<User_Group> userGroups = new List<User_Group> { userGroup };
            bool actualSuccess = false;
            // Act
            if (_groupService.Save(userGroups) > 0)
            {
                actualSuccess = true;
            }

            // Assert
            Assert.AreEqual(expectedSuccess, actualSuccess);
        }

        [Test]
        public async Task CheckIsUserInGroup_AllParamsOk_UserIsInGroup()
        {
            // Arrange
            string emailFirstUser = "test1@gmail.com";                   
            var user = new ApplicationUser { UserName = emailFirstUser, Email = emailFirstUser, FirstName = "John", LastName = "Doe" };
            var user2 = await _userManager.CreateAsync(user);
            var x = await _userManager.FindByIdAsync(user.Id);
         

            var group = new Group { Name = "group 1" };
            _appContext.Groups.Add(group); _appContext.SaveChanges();


            var userGroup = new User_Group { GroupId = group.Id, UserId = user.Id };
            _appContext.User_Groups.Add(userGroup); _appContext.SaveChanges();
            // Act
            bool actualSuccess = await _groupService.CheckIsUserInGroup(user.Id, group.Id);

            // Assert
            Assert.IsTrue(actualSuccess);
        }

        [Test]
        public async Task CheckIsUserInGroup_UserIdNotInUserGroup_UserIsNotInGroup()
        {
            // Arrange
            string emailFirstUser = "test1@gmail.com";
            var user = new ApplicationUser { UserName = emailFirstUser, Email = emailFirstUser, FirstName = "John", LastName = "Doe" };
            await _userManager.CreateAsync(user);

            var group = new Group { Name = "group 1" };
            _appContext.Groups.Add(group); _appContext.SaveChanges();

            var userGroup = new User_Group { GroupId = group.Id, UserId = user.Id + "to be sure its diffrent id" };
            _appContext.User_Groups.Add(userGroup); _appContext.SaveChanges();
            // Act
            bool actualSuccess = await _groupService.CheckIsUserInGroup(user.Id, group.Id);

            // Assert
            Assert.IsFalse(actualSuccess);
        }
/*
        [Test]
        public async Task CheckIsUserInGroup_UserDoesntExist_UserIsNotInGroup()
        {
            // Arrange
            var group = new Group { Name = "group 1" };
            _appContext.Groups.Add(group); _appContext.SaveChanges();

            var userGroup = new User_Group { GroupId = group.Id, UserId = "test ID" };
            _appContext.User_Groups.Add(userGroup); _appContext.SaveChanges();

            // Assert
            Assert.Throws<InvalidOperationException>(() => _groupService.CheckIsUserInGroup("test ID", group.Id));
        }*/
        /*
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
        }*/
        /*
        [Test]
        public async Task SaveUserGroupRequest_AllParamsOk_UserGroupSaved()
        {
            // Arrange
            string emailFirstUser = "test1@gmail.com";
            string emailSecondUser = "test2@gmail.com";
            List<String> emailsOfUsersInDB = new List<String>() { emailFirstUser, emailSecondUser };

            var userGroupRequest = new UserGroupRequest { GroupName = "new group 1", Emails = emailsOfUsersInDB };
            var user1 = new ApplicationUser { UserName = emailFirstUser, Email = emailFirstUser, FirstName = "John", LastName = "Doe" };
            var user2 = new ApplicationUser { UserName = emailSecondUser, Email = emailSecondUser, FirstName = "Mark", LastName = "Snow" };

            await _userManager.CreateAsync(user1);
            await _userManager.CreateAsync(user2);
                       

            // Act
            var success = await _groupService.Save(userGroupRequest);

            // Assert
            Assert.IsTrue(success);

            var users = await _userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                await _userManager.DeleteAsync(user);
            }
        }*/

    }
}

/*
 
  
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
        
 */