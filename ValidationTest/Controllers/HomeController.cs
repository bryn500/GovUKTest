using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ValidationTest.Models.Config;
using ValidationTest.Models.ViewModels;

namespace ValidationTest.Controllers
{
    //[ResponseCache(CacheProfileName = "Default")]
    public class HomeController : BaseController
    {
        private readonly EmailConfig _emailConfig;

        public HomeController(IOptions<EmailConfig> emailConfig)
        {
            _emailConfig = emailConfig.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            SetSEOTags("Home", "Home Description");

            var viewModel = new HomeViewModel() {
                Message = "Gov Template"
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Index(HomeViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);

            SetSEOTags("Home", "Home Description");

            var viewModel = new HomeViewModel()
            {
                Message = "Gov Template"
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            SetSEOTags("Privacy Policy", "Privacy Policy Description");

            return View();
        }
    }
}
