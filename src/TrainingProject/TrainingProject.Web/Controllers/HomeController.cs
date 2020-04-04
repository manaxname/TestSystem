using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using TrainingProject.Common;
using TrainingProject.Domain.Logic.Managers;

namespace TrainingProject.Web.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PartialAction()
        {
            return PartialView();
        }

        [HttpPost]
        public IActionResult Index(string submitButton)
        {
            var a = submitButton;

            return View();
        }
    }
}
