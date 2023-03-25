using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Split_Receipt.Areas.Identity.Data;
using Split_Receipt.Payload;
using Split_Receipt.Services;
using Split_Receipt.Services.Interfaces;

namespace Split_Receipt.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICheckoutService _checkoutService;
        private readonly IGroupService _groupService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(ICheckoutService checkoutService, UserManager<ApplicationUser> userManager, IGroupService group)
        {
            _checkoutService = checkoutService;
            _userManager = userManager;
            _groupService = group;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> getAllCheckouts()
        {
            var allCheckouts = await _checkoutService.getAll();
            return View(allCheckouts);
        }

        //[HttpGet("checkouts/byGroup/{groupId}")]
        [Authorize]
        [HttpGet]
        [Route("Checkout/ByGroup/{groupId}")]
        public async Task<IActionResult> GetAllCheckoutsByGroupId(int groupId)
        {
            var checkouts = await _checkoutService.getAllByGroupID(groupId);
            return View(checkouts);
        }



        [Authorize]
        [HttpGet]
        [Route("Checkout/SaveCheckout/{groupId}")]
        public IActionResult SaveCheckout(int groupId)
        {
            ViewBag.GroupId = groupId;
            return View();
        }

        [Authorize]
        [HttpPost]
        [Route("Checkout/SaveCheckout/{groupId}")]
        public async Task<IActionResult> SaveCheckout(int groupId, CheckoutRequest body)
        {
            // check if group contains user by UserGroupService if not throw an error
            var user = await _userManager.GetUserAsync(User);
            bool isUserInGroup = _groupService.CheckIsUserInGroup(user.Id,groupId);
            if (!isUserInGroup)
            {
                return Forbid();
            }
            int succesful = _checkoutService.save(body, user.Id, groupId);
            if (succesful > 0)
            {
                 return RedirectToAction("GetAllCheckoutsByGroupId", new { groupId = groupId }); 
            }
            return View(body);
        }




        [Authorize]
        [HttpPost]
        [Route("Checkout/Update/{checkoutId}")]
        public async Task<IActionResult> Update(int checkoutId, CheckoutRequest body)
        {
            var user = await _userManager.GetUserAsync(User);
            bool isUserInCheckout = _checkoutService.CheckIsUserInCheckout(user.Id, checkoutId);
            if (!isUserInCheckout)
            {
                return Forbid();
            }
            int succesful = _checkoutService.update(body, checkoutId);
            if (succesful > 0)
            {
               var checkout = await _checkoutService.get(checkoutId);
               return RedirectToAction("GetAllCheckoutsByGroupId", new { groupId = checkout.GroupId });
            }
            return View(body);
        }


        [Authorize]
        [HttpGet]
        [Route("Checkout/Delete/{checkoutId}")]
        public async Task<IActionResult> Delete(int checkoutId)
        {
            // check if group contains user by UserGroupService if not throw an error
            var user = await _userManager.GetUserAsync(User);
            bool isUserInCheckout = _checkoutService.CheckIsUserInCheckout(user.Id, checkoutId);
            if (!isUserInCheckout)
            {
                return Forbid();
            }
            _checkoutService.delete(checkoutId);
            return RedirectToAction("getAllCheckouts");
        }






        [Authorize]
        [HttpGet]
        [Route("Checkout/Update/{checkoutId}")]
        public async Task<IActionResult> Update(int checkoutId)
        {
            var checkout = await _checkoutService.get(checkoutId);
            ViewBag.CheckoutId = checkoutId;
            ViewBag.Checkout = checkout;

            CheckoutRequest request = new CheckoutRequest();
            request.Currency = checkout.Currency;
            request.Description = checkout.Description;
            request.Price = checkout.Price;
            request.IsSplitted = checkout.IsSplitted;
            return View(request);
        }
    }
}
