using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValidationTest.Controllers
{
    public abstract class BaseController : Controller
    {
        public BaseController()
        {
        }

        /// <summary>
        /// Use this override to do work before or after the Controller action has executed
        /// </summary>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);            
        }

        public void SetSEOTags(string title, string description, string keywords = null)
        {
            ViewBag.SEOTitle = title;
            ViewBag.SEODescription = description;
            ViewBag.SEOKeywords = keywords;
        }
    }
}
