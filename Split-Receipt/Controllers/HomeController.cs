using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Split_Receipt.Areas.Identity.Data;
using Split_Receipt.Models;
using System.Diagnostics;

namespace Split_Receipt.Controllers
{
    /// <summary>
    /// Class <c>HomeController</c> is responsible for handling requests related to
    /// the application's home page and privacy policy. It includes three action methods: Index, Privacy, and Error. 
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            this._userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewData["userId"] = _userManager.GetUserId(this.User);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}