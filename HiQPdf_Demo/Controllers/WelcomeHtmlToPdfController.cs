using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HiQPdf_Demo.Controllers
{
    public class WelcomeHtmlToPdfController : Controller
    {
        // GET: WelcomeHtmlToPdf
        public ActionResult Index()
        {
            return View();
        }
    }
}