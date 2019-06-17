using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ValidationTest.Controllers
{
    [Route("error")]
    [ResponseCache(CacheProfileName = "Never")]
    public class ErrorController : BaseController
    {
        public ErrorController()
        {
        }

        [Route("404")]
        public IActionResult PageNotFound()
        {
            return View();
        }

        [Route("500")]
        public IActionResult ServerError()
        {
            return View();
        }
    }
}
