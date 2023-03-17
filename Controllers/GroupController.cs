using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Split_Receipt.Data;
using Split_Receipt.Models;
using Split_Receipt.Payload;
using System.Drawing.Drawing2D;

namespace Split_Receipt.Controllers
{
    public class GroupController : Controller
    {
        private readonly AuthDbContext _appContext;

        public GroupController(AuthDbContext appContext)
        {
            _appContext = appContext;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult List()
        {
            var groups = _appContext.Groups.ToList();
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


            _appContext.Groups.Add(body);
            if (_appContext.SaveChanges() > 0)
            {
                Console.WriteLine("Succes");
            };


            return RedirectToAction("List");
        }












        [HttpGet]
        public IActionResult List2()
        {
            var user_groups = _appContext.User_Groups.ToList();
            return View(user_groups);
        }


        [HttpGet]
        public IActionResult Create2()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create2(UserGroupRequest body)
        {
            String groupName = body.GroupName;
            /*
             STWORZENIE GRUPY
             */
            Group group = new Group();
            group.Name = groupName;
            _appContext.Groups.Add(group);
         
            //mozliwe ze sie tutaj juz dodalo autmatycznie Id do group jak nie to trzeba szuakc ta grupe innymi sposobami

            ICollection<string> memebers = new List<string>();
            foreach (var email in body.emails)
            {
                // znajdz konto po emailu, pobierz jego id
                //w przeciwnym wypadku wyrzuc blad jak nie znajdziesz konta
                //oczywiscie jeszcze walidacja 
                User_Group userGroup = new User_Group(group.Id, email);
                _appContext.User_Groups.Add(userGroup);
            }
            //dodanie jeszcze jednego uzytkopwnika do user_groups -> tego co to stworzyl
            

            if (ModelState.IsValid || String.IsNullOrEmpty(body.GroupName))
            {
                return View(body);
            }


           // _appContext.Groups.Add(body);
            if (_appContext.SaveChanges() > 0)
            {
                Console.WriteLine("Succes");
            };


            return RedirectToAction("List2");
        }














    }
}
