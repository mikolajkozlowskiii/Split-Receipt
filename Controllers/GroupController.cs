using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Split_Receipt.Areas.Identity.Data;
using Split_Receipt.Data;
using Split_Receipt.Models;
using Split_Receipt.Payload;
using Split_Receipt.Services;
using Split_Receipt.Services.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Drawing2D;

namespace Split_Receipt.Controllers
{
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
        public IActionResult GetAllGroups()
        {
            var groups = _groupService.FindAll();
            return View(groups);
        }


        [HttpGet]
        public IActionResult CreateGroup()
        {
            return View();
        }


        [HttpPost]
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
        public async Task<IActionResult> GetAllUserGroups()
        {
            var user_groups = await _groupService.FindAllUserGroups();
            return View(user_groups);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> YourGroups()
        {
            var user = await _userManager.GetUserAsync(User);
            var user_groups = await _groupService.FindAllUserGroupsResponseByUserId(user.Id);
            return View(user_groups);
        }


        [HttpGet]
        public IActionResult CreateUserGroup()
        {
            return View();
        }


        [HttpPost]
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