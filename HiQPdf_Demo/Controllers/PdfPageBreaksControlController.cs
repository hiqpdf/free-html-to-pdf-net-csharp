using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class PdfPageBreaksControlController : Controller
    {
        // GET: PdfPageBreaksControl
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ConvertToPdf(FormCollection collection)
        {
            // create the HTML to PDF converter
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // set a demo serial number
            htmlToPdfConverter.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            // set browser width
            htmlToPdfConverter.BrowserWidth = int.Parse(collection["textBoxBrowserWidth"]);

            // convert HTML to PDF
            byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(collection["textBoxHtmlCode"], collection["textBoxBaseUrl"]);

            FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "PageBreaksControl.pdf";

            return fileResult;
        }
    }
}