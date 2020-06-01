using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class ConversionTriggeringModeController : Controller
    {
        // GET: ConversionTriggeringMode
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

            // set triggering mode; for WaitTime mode set the wait time before convert
            switch (collection["dropDownListTriggeringMode"])
            {
                case "Auto":
                    htmlToPdfConverter.TriggerMode = ConversionTriggerMode.Auto;
                    break;
                case "WaitTime":
                    htmlToPdfConverter.TriggerMode = ConversionTriggerMode.WaitTime;
                    htmlToPdfConverter.WaitBeforeConvert = int.Parse(collection["textBoxWaitTime"]);
                    break;
                case "Manual":
                    htmlToPdfConverter.TriggerMode = ConversionTriggerMode.Manual;
                    break;
                default:
                    htmlToPdfConverter.TriggerMode = ConversionTriggerMode.Auto;
                    break;
            }

            // convert the URL to PDF
            byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(collection["textBoxHtmlCode"], null);

            FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "TriggeringMode.pdf";

            return fileResult;
        }
    }
}