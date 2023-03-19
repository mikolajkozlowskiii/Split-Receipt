using Split_Receipt.Data;
using System.ComponentModel.DataAnnotations;

namespace Split_Receipt.Payload
{
    public class ExistingEmailsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var emailList = (List<string>)value;
            var dbContext = (AuthDbContext)validationContext.GetService(typeof(AuthDbContext));
            int count = 0;


            foreach (var email in emailList)
            {
               
                if (email.Contains(","))
                {
                    List<String> emails = email.Split(",").Select(e => e.Trim()).ToList();
                    foreach(var oneEmail in emails)
                    {
                        var userInDB = dbContext.Users.FirstOrDefault(u => u.Email == email);
                        if (userInDB != null)
                        {
                            count++;
                        }
                       
                    }
                }
                var user = dbContext.Users.FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    count++;
                }
            }
            if(count > 1)
            {
                return ValidationResult.Success;
            }
           
            return new ValidationResult(ErrorMessage);
        }
    }
}
