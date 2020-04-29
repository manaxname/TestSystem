using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TestSystem.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 404:
                    ViewData["ErrorMessage"] = "404 - page not found, sorry, the resource you requested could not be found";
                    ViewData["Path"] = statusCodeResult.OriginalPath;
                    ViewData["QS"] = statusCodeResult.OriginalQueryString;
                    break;

                case 500:
                    ViewData["ErrorMessage"] = "500 - server internal. Oops, something went wrong.";
                    ViewData["Path"] = statusCodeResult.OriginalPath;
                    ViewData["QS"] = statusCodeResult.OriginalQueryString;
                    break;
            }

            return View("NotFound");
        }
    }
}