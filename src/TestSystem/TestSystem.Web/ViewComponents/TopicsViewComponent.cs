using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestSystem.Domain.Logic.Interfaces;
using TestSystem.Domain.Models;
using TestSystem.Web.Models;
using DataTopic = TestSystem.Data.Models.Topic;
using DomainTopic = TestSystem.Domain.Models.Topic;

namespace TestSystem.Web.ViewComponents
{
    public class TopicsViewComponent : ViewComponent
    {
        private readonly ITestManager _testManager;

        private readonly IMapper _mapper;
        public TopicsViewComponent(ITestManager testManager, IMapper mapper)
        {
            _testManager = testManager ?? throw new ArgumentNullException(nameof(testManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IViewComponentResult> InvokeAsync(int? currentTopicId)
        {
            List<TopicModel> topics = (await _testManager.GetTopicsAsync())
                .Select(x => _mapper.Map<TopicModel>(x)).ToList();

            if (currentTopicId != null)
            {
                ViewData["CurrentTopicId"] = currentTopicId;
            }

            return View(topics);
        }
    }
}
