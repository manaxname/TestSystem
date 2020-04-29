using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestSystem.Web.CustomValidtationAttributes
{
    public class CustomEmailAddressAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Regex rgx = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");

            if (!rgx.IsMatch(value.ToString()))
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Please enter a valid email.";
        }
    }
}
