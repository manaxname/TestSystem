using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TestSystem.Web.CustomValidtationAttributes;

namespace TestSystem.Web.Models
{
    public class LoginModel
    {
        [Required]
        [CustomEmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
