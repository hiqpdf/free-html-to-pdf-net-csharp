using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class ConvertHtmlToSvgController : Controller
    {
        // GET: ConvertHtmlToSvg
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ConvertToSvg(FormCollection collection)
        {
            // create the HTML to SVG converter
            HtmlToSvg htmlToSvgConverter = new HtmlToSvg();

            // set a demo serial number
            htmlToSvgConverter.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            // set browser width
            htmlToSvgConverter.BrowserWidth = int.Parse(collection["textBoxBrowserWidth"]);

            // set browser height if specified, otherwise use the default
            if (collection["textBoxBrowserHeight"].Length > 0)
                htmlToSvgConverter.BrowserHeight = int.Parse(collection["textBoxBrowserHeight"]);

            // set HTML Load timeout
            htmlToSvgConverter.HtmlLoadedTimeout = int.Parse(collection["textBoxLoadHtmlTimeout"]);

            // set triggering mode; for WaitTime mode set the wait time before convert
            switch (collection["dropDownListTriggeringMode"])
            {
                case "Auto":
                    htmlToSvgConverter.TriggerMode = ConversionTriggerMode.Auto;
                    break;
                case "WaitTime":
                    htmlToSvgConverter.TriggerMode = ConversionTriggerMode.WaitTime;
                    htmlToSvgConverter.WaitBeforeConvert = int.Parse(collection["textBoxWaitTime"]);
                    break;
                case "Manual":
                    htmlToSvgConverter.TriggerMode = ConversionTriggerMode.Manual;
                    break;
                default:
                    htmlToSvgConverter.TriggerMode = ConversionTriggerMode.Auto;
                    break;
            }

            // convert to SVG
            string svgFileName = "HtmlToSvg.svg";
            byte[] svgBuffer = null;

            if (collection["UrlOrHtmlCode"] == "radioButtonConvertUrl")
            {
                // convert URL
                string url = collection["textBoxUrl"];

                svgBuffer = htmlToSvgConverter.ConvertUrlToMemory(url);
            }
            else
            {
                // convert HTML code
                string htmlCode = collection["textBoxHtmlCode"];
                string baseUrl = collection["textBoxBaseUrl"];

                svgBuffer = htmlToSvgConverter.ConvertHtmlToMemory(htmlCode, baseUrl);
            }

            FileResult fileResult = new FileContentResult(svgBuffer, "image/svg+xml");
            fileResult.FileDownloadName = svgFileName;

            return fileResult;
        }
    }
}