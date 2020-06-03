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
using DomainTopic = TestSystem.Domain.Models.Topic;
using DomainUserTopic = TestSystem.Domain.Models.UserTopic;
using Microsoft.EntityFrameworkCore;

namespace TestSystem.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserManager _userManager;

        private readonly IEmailService _emailService;

        private readonly ITestManager _testManager;

        private readonly IMapper _mapper;

        public AccountController(IUserManager userManager, IEmailService emailService, IMapper mapper, ITestManager testManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _testManager = testManager ?? throw new ArgumentNullException(nameof(testManager));
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
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new
                        {
                            @UserId = domainUser.Id,
                            @ConfirmationToken = domainUser.ConfirmationToken.ToString()
                        },
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

                        List<DomainTopic> domainTopics = await _testManager.GetTopicsAsync(null).ToListAsync();

                        foreach (var domainTopic in domainTopics)
                        {
                            var userTopic = new DomainUserTopic
                            {
                                UserId = userId,
                                TopicId = domainTopic.Id,
                                Status = TopicStatus.NotStarted,
                                IsTopicAsigned = domainTopic.TopicType == TopicType.Public ? true : false,
                                Points = 0
                            };

                            await _testManager.CreateUserTopicAsync(userTopic);
                        }

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

        private async Task Authenticate(DomainUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString()),
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}