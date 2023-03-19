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

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult List()
        {
            var groups = _groupService.GetAll();
            return View(groups);
        }



        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Group body)
        {
            if (ModelState.IsValid || String.IsNullOrEmpty(body.Name))
            {
                return View(body);
            }

            _groupService.Save(body);

            return RedirectToAction("List");
        }







        [HttpGet]
        public async Task<IActionResult> List2()
        {
            var user_groups = await _groupService.GetAllUserGroups();
            return View(user_groups);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> YourGroups()
        {
            var user = await _userManager.GetUserAsync(User);
            var user_groups = await _groupService.FindAllUserGroupsByUserId(user.Id);
            return View(user_groups);
        }


        [HttpGet]
        public IActionResult Create2()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create2(UserGroupRequest body) 
        {
  
            if (!ModelState.IsValid)
            {
                return View(body);
            }

            string emails = body.Emails.FirstOrDefault();
            List<String> emailList = new List<string>();
            if (!String.IsNullOrEmpty(emails))
            {
                emailList = emails.Split(",").Select(e => e.Trim()).ToList();
            }
            

            var modifiedRequest = new UserGroupRequest(body.GroupName, emailList);

            
            var loggedUserEmail = User.Identity.Name;
            var succesful = await _groupService.Save(modifiedRequest, loggedUserEmail);

            if (succesful)
            {
                return RedirectToAction("YourGroups");
            }
            return View(body);
        }
    }
}
//TODO
//REPAIR CREATE 2 (FIRST COMMENT)
//SHOULD BE [AUTHORIZE] AND RETURN ONLY GROUPS WHERE YOU BELONG TO AND NAME SHOULD BE: YOUR GROUPS
//ADD VALIDATION IN VIEW, TO USER'S KNOW WHAT IS HE DOING WRONG

///*
///wchodzisz na groups i przechodzisz do details -> tam do kazdej grupy wyswietla sie paragony
//czyli kontroler ktory jako argument pobiera group Id (do userGroupResponse przekazywac Id ale nie wyswietlac w stringu?) i wyswietla wszystkie paragony gdzie groupId = sie wlasnie to groupId
//na gorze w zaleznosc od tego jakim userem jestes bedzie pokazane czy jestes na plusie czy na minusie i ile i domyslna waluta zlotowki, ale bedzie mozna zmienic
/// 
/// */