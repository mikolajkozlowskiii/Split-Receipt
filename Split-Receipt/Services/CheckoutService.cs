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
    /// The <c>CheckoutService</c> class is an implementation of the <t>ICheckoutService</t> interface.
    /// It provides functionality for saving, updating, deleting and finding Checkout objects in the database.
    /// The class contains a constructor that initializes the necessary dependencies such as <c>AuthDbContext</c>,
    /// <c>UserManager<ApplicationUser></c>, <c>IGroupService</c>, <c>ICurrencyService</c> and <c>CheckoutMapper</c>.
    public class CheckoutService : ICheckoutService
    {
        private readonly AuthDbContext _appContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGroupService _groupService;
        private readonly ICurrencyService _currencyService;
        private readonly CheckoutMapper _checkoutMapper;

        public CheckoutService(AuthDbContext appContext, UserManager<ApplicationUser> userManager,
            IGroupService groupService, ICurrencyService currencyService, CheckoutMapper checkoutMapper)
        {
            _appContext = appContext;
            _userManager = userManager;
            _groupService = groupService;
            _currencyService = currencyService;
            _checkoutMapper = checkoutMapper;
        }

        /// <summary>
        /// This method saves checkout in DB.
        /// </summary>
        /// <param name="requestCheckout"></param> is an request with body of information about saved checkout from user.
        /// <param name="userId"></param> is an user's id who is requesting save his checkout in DB.
        /// Saved Checkout's object also contains data about user's id.
        /// <param name="groupId"></param> is an group's id which checkout's belongs. 
        /// Saved Checkout's object also containts data about group's id.
        /// <returns>Positive integer number if DB saved changes. Otherwise negative number.</returns>
        public async Task<int> Save(CheckoutRequest requestCheckout, string userId, int groupId)
        {
            /*  var isUserInGroup = await _groupService.CheckIsUserInGroup(userId, groupId);
              if (!isUserInGroup)
              {
                  throw new InvalidOperationException("User is not in group.");
              }*/

            bool isValid = Validation(requestCheckout);
            if (!isValid)
            {
                throw new InvalidOperationException("Empty or null required fields in RequestCheckout obejct.");
            }
            Checkout checkout = new Checkout
            {
                Currency = requestCheckout.Currency.ToUpper(),
                Price = requestCheckout.Price,
                Description = requestCheckout.Description,
                IsSplitted = requestCheckout.IsSplitted,
                UserId = userId,
                GroupId = groupId,
                CreatedAt = DateTime.Now
            };
            _appContext.Checkouts.Add(checkout);

            return _appContext.SaveChanges();
        }

        private static bool Validation(CheckoutRequest requestCheckout)
        {
            var context = new ValidationContext(requestCheckout, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(requestCheckout, context, validationResults, true);
            return isValid;
        }

        /// <summary>
        /// This method updates Checkout's object instance in DB.
        /// </summary>
        /// <param name="checkoutRequest"></param> is a body with updates.
        /// <param name="checkoutId"></param> is a id of checkout which is updated.
        /// <returns>Positive integer if update operation ended successfully, otherwise negative number</returns>
        public int Update(CheckoutRequest checkoutRequest, int checkoutId)
        {
            var checkout = _appContext.Checkouts.Find(checkoutId);
            if (checkout == null)
            {
                throw new InvalidOperationException("Checkout id: " + checkoutId + " not founded in DB.");
            }
            checkout.Price = checkoutRequest.Price;
            checkout.Description = checkoutRequest.Description;
            checkout.Currency = checkoutRequest.Currency.ToUpper();
            checkout.IsSplitted = checkoutRequest.IsSplitted;

            _appContext.SaveChanges();

            return checkout.Id;
        }

        /// <summary>
        /// This method deletes from DB instance of Checkout's object.
        /// </summary>
        /// <param name="checkoutId"></param> is an id of target Checkout's object removed.
        /// <returns>Positive number if Delete operation ran successfully,
        /// otherwise non-positive number.</returns>
        public int Delete(int checkoutId)
        {
            var checkout = _appContext.Checkouts.Find(checkoutId);
             if (checkout == null)
            {
                throw new InvalidOperationException("Checkout id: " + checkoutId + " not founded in DB.");
            }
            _appContext.Checkouts.Remove(checkout);
            return _appContext.SaveChanges(); 
        }

        /// <summary>
        /// This method returns mapped response of Checkout's instance in DB.
        /// </summary>
        /// <param name="checkoutId"></param> is and id of target returned Checkout's object.
        /// <returns>Mapped Checkout from DB on CheckoutResponse.</returns>
        /// <exception cref="InvalidOperationException">Thrown when there is no checkout in DB with specific checkoutId from param</exception>
        public async Task<CheckoutResponse> FindById(int checkoutId)
        {
            var checkout = _appContext.Checkouts.FirstOrDefault(x => x.Id == checkoutId);
            if(checkout == null)
            {
                throw new InvalidOperationException("There is no checkout in DB with id: " + checkoutId);
            }
            var user = await _userManager.FindByIdAsync(checkout.UserId);
    
            CheckoutResponse response =  _checkoutMapper.map(checkout, user.Email);
            return response;
        }

        /// <summary>
        /// This method finds all Checkout's object instances saved in DB.
        /// </summary>
        /// <returns>List of mapped Checkout's objects to CheckoutResponse's objects</returns>
        public async Task<List<CheckoutResponse>> FindAll()
        {
            var checkouts = _appContext.Checkouts.ToList();
            return await _checkoutMapper.map(checkouts);
        }

        /// <summary>
        /// This method finds all Checkout's object instances saved in DB,
        /// which belongs to specific group.
        /// </summary>
        /// <param name="groupId"></param> is an id to recognize group where checkouts belongs. 
        /// <param name="sortBy"></param> is an sort type by which list of checkouts are sorted.
        /// It's also parameter of private method Sort which this method uses to sort objects.
        /// <returns>List of mapped Checkout's objects to CheckoutResponse's objects and also sorted
        /// by sortBy param.</returns>
        public async Task<List<CheckoutResponse>> FindlAllByGroupId(int groupId, string sortBy)
        {
            var checkouts = _appContext.Checkouts
                .Where(x => x.GroupId == groupId)
                .ToList();
            List<CheckoutResponse> responseList = await _checkoutMapper.map(checkouts);
            return await Sort(sortBy, responseList);
        }

        /// <summary>
        /// This method finds all checkouts which belong to specific user.
        /// </summary>
        /// <param name="userId"></param> is an id of user.
        /// <returns>List of mapped Checkout's objects to CheckoutResponse's objects.</returns>
        public async Task<List<CheckoutResponse>> FindAllByUserID(string userId)
        {
            var checkouts = _appContext.Checkouts
                .Where(x => x.UserId == userId)
                .ToList();
            return await _checkoutMapper.map(checkouts);
        }

        /// <summary>
        /// This method informs if specific checkout belongs to specific user.
        /// </summary>
        /// <param name="userId"></param> is id of checked user.
        /// <param name="checkoutId"></param> is id of checked checkout.
        /// <returns>True if checkout belongs to user, else false</returns>
        public bool CheckIsUserInCheckout(string userId, int checkoutId)
        {
            return _appContext.Checkouts.Any(x => x.UserId == userId && x.Id == checkoutId);
        }

        /// <summary>
        /// This method creates the most important view of checkouts for specific group.
        /// The view is in instance of CheckoutSummary object, it contains all necessery informations like
        /// balance with specific currency, members of group, name of group. It is also sorted in specific way.
        /// </summary>
        /// <param name="userEmail"></param> is an email of user to recognize who is viewing this CheckoutSummary. It is important
        /// who is watching it, because total amount depends on for who is that view.
        /// <param name="quoteCurrency"></param> is a currency in which is showing balance.
        /// <param name="groupId"></param> is an id of group to which this view belongs
        /// <param name="sortBy"></param> is an way of sorting by which the checkouts are sorted
        /// <returns>CheckoutSummary object instance.</returns>
        public async Task<CheckoutSummary> CreateCheckoutSummary(string userEmail, string quoteCurrency, int groupId, string sortBy)
        {
            List<string> members = await _groupService.GetAllMembersEmails(groupId);
            List<CheckoutResponse> allCheckouts = await FindlAllByGroupId(groupId, sortBy);
            var groupName = _groupService.FindById(groupId).Name;
            int numOfMemebers = members.Count();
            decimal total = await ComputeTotalBalance(userEmail, quoteCurrency, numOfMemebers, allCheckouts);

            CheckoutSummary checkoutSummary = new CheckoutSummary
            {
                Email = userEmail,
                GroupName = groupName,
                Checkouts = allCheckouts,
                Total = total,
                Currency = quoteCurrency.ToUpper(),
                Members = members,
            };

            return checkoutSummary;
        }

        /// <summary>
        /// This method sorts a list of CheckoutResponse objects based on the provided sort criteria.
        /// If the sortBy parameter is set to "Price ASC" or "Price DESC", it calculates the equivalent price of each checkout using the GetCheckoutEquivalentPriceDict() method,
        /// and sorts the dictionary based on the calculated prices. The method then returns the sorted dictionary's keys as a list of CheckoutResponse objects.
        /// If the sortBy parameter is set to "Date ASC" or "Date DESC", it sorts the responseList directly based on the CreatedAt property.
        /// If the sortBy parameter is not recognized, it sorts the responseList based on CreatedAt by default.
        /// </summary>
        /// <param name="sortBy">The sort criteria</param>
        /// <param name="responseList">The list of CheckoutResponse objects to be sorted</param>
        /// <returns>A sorted list of CheckoutResponse objects</returns>
        public async Task<List<CheckoutResponse>> Sort(string sortBy, List<CheckoutResponse> responseList)
        {
            Dictionary<CheckoutResponse, decimal> checkoutDict;
            switch (sortBy)
            {
                case "Price ASC":
                    checkoutDict = await GetCheckoutEquivalentPriceDict(responseList);
                    var sortedDict = from entry in checkoutDict orderby entry.Value ascending select entry;
                    return sortedDict.Select(entry => entry.Key).ToList();

                case "Price DESC":
                    checkoutDict = await GetCheckoutEquivalentPriceDict(responseList);
                    var sortedDict2 = from entry in checkoutDict orderby entry.Value descending select entry;
                    return sortedDict2.Select(entry => entry.Key).ToList();

                case "Date ASC":
                    responseList.Sort((x, y) => x.CreatedAt.CompareTo(y.CreatedAt));
                    break;

                case "Date DESC":
                    responseList.Sort((x, y) => y.CreatedAt.CompareTo(x.CreatedAt));
                    break;

                default:
                    responseList.Sort((x, y) => x.CreatedAt.CompareTo(y.CreatedAt));
                    break;
            }

            return responseList;
        }

        private async Task<Dictionary<CheckoutResponse, decimal>> GetCheckoutEquivalentPriceDict(List<CheckoutResponse> responseList)
        {
            var quoteCurrency = "PLN";
            Dictionary<CheckoutResponse, decimal> checkoutDict = new Dictionary<CheckoutResponse, decimal>();
            foreach (var checkoutResponse in responseList)
            {
                decimal priceInSameCurrency = 0;
                if (checkoutResponse.Price != 0)
                {
                    priceInSameCurrency = await (_currencyService.GetRate(checkoutResponse.Currency, quoteCurrency)) * (checkoutResponse.Price);
                } 
                checkoutDict.Add(checkoutResponse, priceInSameCurrency);
            }
            return checkoutDict;
        }

        /// <summary>
        /// Computes the total balance for a user within a group based on their checkouts and the number of group members.
        /// </summary>
        /// <param name="userEmail">The email of the user to compute the balance for.</param>
        /// <param name="quoteCurrency">The currency to convert the checkouts to for computing the balance.</param>
        /// <param name="numOfMemebers">The number of members in the group.</param>
        /// <param name="checkouts">The list of checkouts for the group.</param>
        /// <returns>The total balance for the user in the specified currency.</returns>
        public async Task<decimal> ComputeTotalBalance(string userEmail, string quoteCurrency, int numOfMemebers, List<CheckoutResponse> checkouts)
        {
            if(numOfMemebers < 2)
            {
                throw new InvalidOperationException("Group must have at least 2 memebers.");
            }

            decimal total = 0;
            foreach (var checkout in checkouts)
            {
                decimal rate = 1;
                if (!quoteCurrency.Equals(checkout.Currency))
                {
                    rate = await _currencyService.GetRate(checkout.Currency, quoteCurrency);
                }
                decimal currentPrice = checkout.Price;
                if (checkout.IsSplitted)
                {
                    currentPrice = currentPrice / numOfMemebers;
                }
                else if(!checkout.IsSplitted && !userEmail.Equals(checkout.UserEmail)) 
                {
                    currentPrice = currentPrice / (numOfMemebers - 1);
                }
                if (userEmail.Equals(checkout.UserEmail))
                {
                    total = total + currentPrice * rate;
                }
                else
                {
                    total = total - currentPrice * rate;
                }

            }
            return total;
        }
    }
}
