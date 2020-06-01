using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HiQPdf_Demo.Controllers
{
    public class WelcomePdfFormsController : Controller
    {
        // GET: WelcomePdfForms
        public ActionResult Index()
        {
            return View();
        }
    }
}