using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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

        private readonly IEmailService _emailService;

        private readonly IMapper _mapper;

        public AccountController(IUserManager userManager, IEmailService emailService, IMapper mapper)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
                        DomainUser domainUser = await _userManager.CreateUserAsync(model.Email, model.Password, UserRoles.User); // token was generated automatically

                        string subject = "Test System";
                        string senderName = "Test System Administration";
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { @UserId = domainUser.Id, @ConfirmationToken = domainUser.ConfirmationToken.ToString() },
                            protocol: HttpContext.Request.Scheme);

                        string messgae = $"Confirm your account by clicking: <a href='{callbackUrl}'>link</a>";

                        _emailService.SendEmailAsync(senderName, WebExtensions.SenderEmail, WebExtensions.SenderEmailPassword, WebExtensions.SmtpHost,
                            WebExtensions.SmtpPort, model.Email, subject, messgae); // not awaiting

                        return RedirectToAction("ConfirmEmailPage", "Account", new { @userId = domainUser.Id });
                    }
                    catch (UserAlreadyExistsException)
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
        public async Task<IActionResult> ConfirmEmailPage(int userId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var domainUser = await _userManager.GetUserByIdAsync(userId);

                if (!domainUser.IsConfirmed)
                {
                    return View(_mapper.Map<UserModel>(domainUser));
                }
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(int userId, string confirmationToken)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var domainUser = await _userManager.GetUserByIdAsync(userId);

                if (!domainUser.IsConfirmed)
                {
                    if (domainUser.ConfirmationToken.ToString() == confirmationToken)
                    {
                        await _userManager.UpdateUserConfirmStatus(userId, true);

                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        return RedirectToAction("ConfirmEmailPage", "Account", new { @UserId = domainUser.Id });
                    }
                }
            }

            return BadRequest();
        }


        [HttpGet]
        public IActionResult Login()
        {
            if (!User.Identity.IsAuthenticated)
            { 
                return View();
            }

            return BadRequest();
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

                        if (!domainUser.IsConfirmed)
                        {
                            return RedirectToAction("ConfirmEmailPage", "Account", new { @UserId = domainUser.Id });
                        }

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


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> OnPostCaptcha()
        //{
        //    string recaptchaResponse = Request.Form["g-recaptcha-response"];
        //    var client = new HttpClient();
        //    try
        //    {
        //        var parameters = new Dictionary<string, string>
        //        {
        //            {"secret", "6LfB7vEUAAAAAKyhMP2HE-Jgkk2LgKvJqn_i2cYG"},
        //            {"response", recaptchaResponse},
        //            {"remoteip", HttpContext.Connection.RemoteIpAddress.ToString()}
        //        };

        //        HttpResponseMessage response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(parameters));
        //        response.EnsureSuccessStatusCode();

        //        string apiResponse = await response.Content.ReadAsStringAsync();
        //        dynamic apiJson = JObject.Parse(apiResponse);
        //        if (apiJson.success != true)
        //        {
        //            ModelState.AddModelError(string.Empty, "There was an unexpected problem processing this request. Please try again.");
        //        }
        //    }
        //    catch
        //    {
        //        BadRequest();
        //    }

        //    string subject = "Test System";
        //    string senderName = "Test System Administration";
        //    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { @UserId = domainUser.Id, @ConfirmationToken = domainUser.ConfirmationToken.ToString() },
        //        protocol: HttpContext.Request.Scheme);

        //    string messgae = $"Confirm your account by clicking: <a href='{callbackUrl}'>link</a>";

        //    _emailService.SendEmailAsync(senderName, WebExtensions.SenderEmail, WebExtensions.SenderEmailPassword, WebExtensions.SmtpHost,
        //                   WebExtensions.SmtpPort, model.Email, subject, messgae); // not awaiting

        //    return RedirectToAction("ConfirmEmail", "Account", new { @userId = domainUser.Id });
        //}

        private async Task Authenticate(DomainUser user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString()),
            };

            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}