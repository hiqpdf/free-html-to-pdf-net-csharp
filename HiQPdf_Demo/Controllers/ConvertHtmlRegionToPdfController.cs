using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class ConvertHtmlRegionToPdfController : Controller
    {
        // GET: ConvertHtmlRegionToPdf
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

            // select the HTML element to be converted to PDF
            htmlToPdfConverter.ConvertedHtmlElementSelector = collection["textBoxConvertedHtmlElementSelector"];

            // optionally wait an additional time before starting the conversion
            // it is not necessary to set this property if the HTML page is not updated after initial load
            htmlToPdfConverter.WaitBeforeConvert = 2;

            // convert URL to a PDF memory buffer
            string url = collection["textBoxUrl"];

            byte[] pdfBuffer = htmlToPdfConverter.ConvertUrlToMemory(url);

            FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "ConvertHtmlPart.pdf";

            return fileResult;
        }
    }
}