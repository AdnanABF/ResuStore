using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ResuStore.Data.Attributes
{
    public class DateNotInFutureAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || !DateTime.TryParse(value.ToString(), out _))
            {
                return new ValidationResult("Invalid Date of Birth");
            }
            else
            {
                return DateTime.Parse(value.ToString()) > DateTime.Now ? new ValidationResult("Date of Birth cannot be in the future") : ValidationResult.Success;
            }
        }
    }
}