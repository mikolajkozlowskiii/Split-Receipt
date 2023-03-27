using Microsoft.AspNetCore.Identity;
using Split_Receipt.Areas.Identity.Data;
using Split_Receipt.Data;
using System.ComponentModel.DataAnnotations;

namespace Split_Receipt.Payload
{
    public class ExistingEmailsAttribute : ValidationAttribute
    {
        private readonly int MIN_EXISTING_EMAILS = 2;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExistingEmailsAttribute(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

       /* protected override async Task<ValidationResult> IsValid(object value, ValidationContext validationContext)
        {
            var dbContext = (AuthDbContext)validationContext.GetService(typeof(AuthDbContext));
            var emails = (List<string>)value;
            int count = 0;
            HashSet<string> existingEmails = new HashSet<string>();

            if (emails != null && emails.Any())
            {
                foreach (var email in emails)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user != null)
                    {
                        existingEmails.Add(email);
                    }
                }
            }

            if (existingEmails.Count >= MIN_EXISTING_EMAILS)
            {
                return ValidationResult.Success;
            }
            throw new ValidationException("Number of existing emails: " + existingEmails.Count + " don't allow you to create a group.");
        }*/
    }
}
