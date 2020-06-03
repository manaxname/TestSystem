using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestSystem.Common;
using TestSystem.Data.Models;
using TestSystem.Domain.Logic.Interfaces;
using TestSystem.Web.Models;

namespace TestSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IMapper _mapper;

        private readonly ITestManager _testManager;

        private readonly IUserManager _userManager;

        public HomeController(ILogger<HomeController> logger, IMapper mapper, ITestManager testManager, IUserManager userManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _testManager = testManager ?? throw new ArgumentNullException(nameof(testManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index(string search, TopicType topicType, int page = 1, int size = 5)
        {
            int fromIndex = (page - 1) * size;
            int toIndex = fromIndex + size - 1;
            List<TopicModel> topics = null;
            int topicsCount = 0;

            if (User.IsInRole(UserRoles.Admin.ToString()))
            {
                topics = await _testManager.GetTopicsAsync(search, fromIndex, toIndex, null, topicType)
                    .ProjectTo<TopicModel>(_mapper.ConfigurationProvider).ToListAsync();

                topicsCount = await _testManager.GetTopicsCountAsync(search, null, topicType);
            }
            else if (User.IsInRole(UserRoles.User.ToString()))
            {
                int userId = await _userManager.GetUserIdAsync(User.Identity.Name);
                topics = await _testManager.GetUserTopicsAsync(userId, search, fromIndex, toIndex, false, topicType)
                    .ProjectTo<TopicModel>(_mapper.ConfigurationProvider).ToListAsync();

                topicsCount = await _testManager.GetTopicsCountAsync(search, false, topicType);
            }

            ViewData["TopicType"] = topicType;
            ViewData["Page"] = page;
            ViewData["Search"] = search == null ? string.Empty : search;
            ViewData["Size"] = size;
            ViewData["TopicsCount"] = topicsCount;

            return View(topics);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> IndexAjax(string search, TopicType topicType, int page = 1, int size = 5)
        {
            int fromIndex = (page - 1) * size;

            int toIndex = fromIndex + size - 1;
            List<TopicModel> topics = null;
            int topicsCount = 0;

            if (User.IsInRole(UserRoles.Admin.ToString()))
            {
                topics = await _testManager.GetTopicsAsync(search, fromIndex, toIndex, null, topicType)
                    .ProjectTo<TopicModel>(_mapper.ConfigurationProvider).ToListAsync();

                topicsCount = await _testManager.GetTopicsCountAsync(search, null, topicType);
            }
            else if (User.IsInRole(UserRoles.User.ToString()))
            {
                int userId = await _userManager.GetUserIdAsync(User.Identity.Name);
                topics = await _testManager.GetUserTopicsAsync(userId, search, fromIndex, toIndex, false, topicType)
                     .ProjectTo<TopicModel>(_mapper.ConfigurationProvider).ToListAsync();

                topicsCount = await _testManager.GetTopicsCountAsync(search, false, topicType);
            }

            ViewData["TopicType"] = topicType;
            ViewData["Page"] = page;
            ViewData["Search"] = search == null ? string.Empty : search;
            ViewData["Size"] = size;
            ViewData["TopicsCount"] = topicsCount;

            return PartialView("_Topics", topics);
        }
    }
}
