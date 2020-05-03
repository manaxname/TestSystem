using PaulMiami.AspNetCore.Mvc.Recaptcha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestSystem.Web.AppOptions
{
    public class ReCaptchaOptions
    {
        public string SiteKey { get; set; }

        public string SecretKey { get; set; }
    }
}
