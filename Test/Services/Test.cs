using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
    [TestClass]
    public class GroupServiceTests
    {
        private AuthDbContext _appContext;
        private UserManager<ApplicationUser> _userManager;
        private IGroupService _groupService;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<AuthDbContext>()
                .UseInMemoryDatabase(databaseName: "SplitReceipt")
                .Options;

            _appContext = new AuthDbContext(options);

            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new UserManager<ApplicationUser>(store.Object, null, null, null, null, null, null, null, null);

            _groupService = new GroupService(_appContext, _userManager);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _appContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public void TestFindById()
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

        [TestMethod]
        public void TestFindAll()
        {
            // Arrange
            var group1 = new Group { Name = "Group 1" };
            var group2 = new Group { Name = "Group 2" };
            _appContext.Groups.Add(group1);
            _appContext.Groups.Add(group2);
            _appContext.SaveChanges();

            // Act
            var result = _groupService.FindAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }
    }
}