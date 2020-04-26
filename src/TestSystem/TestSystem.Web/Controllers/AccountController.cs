using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestSystem.Common;
using TestSystem.Common.CustomExceptions;
using TestSystem.Domain.Logic.Interfaces;
using TestSystem.Web.Models;
using DomainUser = TestSystem.Domain.Models.User;

namespace TestSystem.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserManager _userManager;
        public AccountController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!User.Identity.IsAuthenticated)
            { 
                if (ModelState.IsValid)
                {
                    try
                    {
                        await _userManager.CreateUserAsync(model.Email, model.Password, UserRoles.User);

                        return RedirectToAction("Login", "Account");
                    }
                    catch(UserAlreadyExistsException)
                    {
                        ModelState.AddModelError("", "User with this email already exists");

                        return View(model);
                    }
                }

                ModelState.AddModelError("", "Invalid Login or(and) Password");

                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (!User.Identity.IsAuthenticated)
            { 
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var domainUser = await _userManager.GetUserByEmailAsync(model.Email);

                        bool isUserValid = _userManager.ValidateUserPassword(domainUser, model.Password);

                        if (isUserValid)
                        {
                            await Authenticate(domainUser);

                            return RedirectToAction("Index", "Home");
                        }
                    }
                    catch(UserNotFoundException)
                    {
                        ModelState.AddModelError("", "Invalid Login or(and) Password, try again");

                        return View(model);
                    }
                }

                ModelState.AddModelError("", "Invalid Login or(and) Password");

                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOff()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        private async Task Authenticate(DomainUser user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role),
            };

            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}