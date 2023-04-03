using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Split_Receipt.Areas.Identity.Data;
using Split_Receipt.Models;
using Split_Receipt.Payload;
using Split_Receipt.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Split_Receipt.Controllers
{

    /// <summary>
    /// Class <c>GroupController</c> is responsible for handling group-related actions
    /// in the Split Receipt application. It depends on two services: IGroupService,
    /// which provides group-related functionality, and UserManager<ApplicationUser>,
    /// which manages user information
    /// </summary>
    public class GroupController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly UserManager<ApplicationUser> _userManager;

        public GroupController(IGroupService groupService, UserManager<ApplicationUser> userManager)
        {
            _groupService = groupService;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAllGroups()
        {
            var groups = _groupService.FindAll();
            return View(groups);
        }


        [HttpGet]
        [Authorize]
        public IActionResult CreateGroup()
        {
            return View();
        }


        [HttpPost]
        [Authorize]
        public IActionResult CreateGroup(Group body)
        {
            if (ModelState.IsValid || String.IsNullOrEmpty(body.Name))
            {
                return View(body);
            }

            _groupService.Save(body);

            return RedirectToAction("List");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllUserGroups()
        {
            var user_groups = await _groupService.FindAllUserGroups();
            return View(user_groups);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> YourGroups()
        {
            var user = await _userManager.GetUserAsync(User);
            var user_groups = await _groupService.FindAllUserGroupsResponseByUserId(user.Id);
            return View(user_groups);
        }


        [HttpGet]
        [Authorize]
        public IActionResult CreateUserGroup()
        {
            return View();
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateUserGroup(UserGroupRequest body) 
        {
            body.Emails.Add(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return View(body);
            }

            try
            {
                await _groupService.Save(body);
                return RedirectToAction("YourGroups");
            }
            catch(ValidationException ex)
            {
                return View(body);
            }
        }
    }
}