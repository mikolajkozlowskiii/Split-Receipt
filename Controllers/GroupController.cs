using Azure.Core;
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

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
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


        [HttpGet]
        public IActionResult Create2()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create2(UserGroupRequest body) // SHOULD CHECK _groupService.Save(modifiedRequest, loggedUserEmail); IS >=2 MEMBERS IN GROUP, IF NOT SHOULD RETURN BOOLEAN AND THERE WE SHOULD THROW PROPER ACTION
        {
            //string[] emailsArray = userGroupRequest.emails.Split(",");
            //ICollection<string> emailsList = new List<string>(emailsArray);
            string emails = body.Emails.FirstOrDefault();

            var emailList =emails.Split(",").Select(e => e.Trim()).ToList();

            // tworzenie nowego obiektu UserGroupRequest z poprawioną listą emaili
            var modifiedRequest = new UserGroupRequest(body.GroupName, emailList);
            foreach (var email in modifiedRequest.Emails)
            if (!ModelState.IsValid)
            {
                return View(body);
            }
            var loggedUserEmail = User.Identity.Name;
            _groupService.Save(modifiedRequest, loggedUserEmail);


            return RedirectToAction("List2");
        }


    }
}
//TODO
//REPAIR CREATE 2 (FIRST COMMENT)
//SHOULD BE [AUTHORIZE] AND RETURN ONLY GROUPS WHERE YOU BELONG TO AND NAME SHOULD BE YOUR GROUPS
//ADD VALIDATION IN VIEW, TO USER'S KNOW WHAT IS HE DOING WRONG
