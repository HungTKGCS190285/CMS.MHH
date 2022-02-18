using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.MHH.Controllers
{
    public class IdeasController : Controller
    {
        // GET: Idea
        public ActionResult Index()
        {
            return View();
        }
    }
}