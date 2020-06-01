using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class WebFontsController : Controller
    {
        // GET: WebFonts
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConvertToPdf(FormCollection collection)
        {
            // create the HTML to PDF converter
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // set a demo serial number
            htmlToPdfConverter.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            // optionally wait an additional time before starting the conversion
            // it is not necessary to set this property if the HTML page is not updated after initial load
            htmlToPdfConverter.WaitBeforeConvert = 3;

            byte[] pdfBuffer = htmlToPdfConverter.ConvertUrlToMemory(collection["textBoxUrl"]);

            FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "WebFonts.pdf";

            return fileResult;
        }
    }
}