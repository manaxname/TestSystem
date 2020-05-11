using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TestSystem.Domain.Logic.Interfaces;
using TestSystem.Web.Models;

namespace TestSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IMapper _mapper;

        private readonly ITestManager _testManager;

        public HomeController(ILogger<HomeController> logger, IMapper mapper, ITestManager testManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _testManager = testManager ?? throw new ArgumentNullException(nameof(testManager));
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? currentTopicId, string search, int page = 1, int size = 5)
        {
            int fromIndex = (page - 1) * size;
            int toIndex = fromIndex + size;

            List<TopicModel> topics = (await _testManager.GetTopicsAsync(search, fromIndex, toIndex))
                .Select(x => _mapper.Map<TopicModel>(x)).ToList();

            int topicsCount = topics.Count;

            if (currentTopicId != null)
            {
                ViewData["CurrentTopicId"] = currentTopicId;
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                ViewData["Search"] = search;
            }

            ViewData["Page"] = page;
            ViewData["Size"] = size;
            ViewData["TopicsCount"] = topicsCount;

            return View(topics);
        }


        [HttpGet]
        public async Task<IActionResult> IndexAjax(int? currentTopicId, string search, int page = 1, int size = 5)
        {
            int fromIndex = (page - 1) * size;
            int toIndex = fromIndex + size;

            List<TopicModel> topics = (await _testManager.GetTopicsAsync(search, fromIndex, toIndex))
                .Select(x => _mapper.Map<TopicModel>(x)).ToList();

            int topicsCount = topics.Count;

            if (currentTopicId != null)
            {
                ViewData["CurrentTopicId"] = currentTopicId;
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                ViewData["Search"] = search;
            }

            ViewData["Page"] = page;
            ViewData["Size"] = size;
            ViewData["TopicsCount"] = topicsCount;

            return PartialView("_Topics", topics);
        }
    }
}
