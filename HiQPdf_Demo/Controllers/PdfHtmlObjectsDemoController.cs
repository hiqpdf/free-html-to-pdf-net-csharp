using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class PdfHtmlObjectsDemoController : Controller
    {
        // GET: PdfHtmlObjectsDemo
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreatePdf(FormCollection collection)
        {
            // create an empty PDF document
            PdfDocument document = new PdfDocument();

            // set a demo serial number
            document.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            // add a page to document
            PdfPage page1 = document.AddPage(PdfPageSize.A4, new PdfDocumentMargins(0), PdfPageOrientation.Portrait);

            // an object to be set with HTML layout info after conversion
            PdfLayoutInfo htmlLayoutInfo = null;
            try
            {
                // create the HTML object from URL or HTML code
                PdfHtml htmlObject = null;
                if (collection["UrlOrHtmlCode"] == "radioButtonConvertUrl")
                {
                    // create from URL
                    htmlObject = new PdfHtml(collection["textBoxUrl"]);
                }
                else
                {
                    // create from HTML code
                    string htmlCode = collection["textBoxHtmlCode"];
                    string baseUrl = collection["textBoxBaseUrl"];

                    htmlObject = new PdfHtml(htmlCode, baseUrl);
                }

                // set the HTML object start location in PDF page
                htmlObject.DestX = float.Parse(collection["textBoxDestX"]);
                htmlObject.DestY = float.Parse(collection["textBoxDestY"]);

                // set the HTML object width in PDF
                if (collection["textBoxDestWidth"].Length > 0)
                    htmlObject.DestWidth = float.Parse(collection["textBoxDestWidth"]);

                // set the HTML object height in PDF
                if (collection["textBoxDestHeight"].Length > 0)
                    htmlObject.DestHeight = float.Parse(collection["textBoxDestHeight"]);

                // optionally wait an additional time before starting the conversion
                htmlObject.WaitBeforeConvert = 2;

                // set browser width
                htmlObject.BrowserWidth = int.Parse(collection["textBoxBrowserWidth"]);

                // set browser height if specified, otherwise use the default
                if (collection["textBoxBrowserHeight"].Length > 0)
                    htmlObject.BrowserHeight = int.Parse(collection["textBoxBrowserHeight"]);

                // set HTML load timeout
                htmlObject.HtmlLoadedTimeout = int.Parse(collection["textBoxLoadHtmlTimeout"]);

                // layout the HTML object in PDF
                htmlLayoutInfo = page1.Layout(htmlObject);

                // write the PDF document to a memory buffer
                byte[] pdfBuffer = document.WriteToMemory();

                FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "PdfHtmlObjects.pdf";

                return fileResult;
            }
            finally
            {
                document.Close();
            }
        }
    }
}