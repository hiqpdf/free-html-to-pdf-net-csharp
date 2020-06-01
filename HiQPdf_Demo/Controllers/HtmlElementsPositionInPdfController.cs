using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class HtmlElementsPositionInPdfController : Controller
    {
        // GET: HtmlElementsPositionInPdf
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConvertToPdf(FormCollection collection)
        {
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // set a demo serial number
            htmlToPdfConverter.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            htmlToPdfConverter.SelectedHtmlElements = new string[] { collection["textBoxImageSelector"] };
            htmlToPdfConverter.HiddenHtmlElements = new string[] { collection["textBoxImageSelector"] };

            PdfDocument document = null;
            try
            {
                document = htmlToPdfConverter.ConvertUrlToPdfDocument(collection["textBoxUrl"]);

                foreach (HtmlElementInfo htmlImageInfo in htmlToPdfConverter.ConversionInfo.SelectedHtmlElementsInfo)
                {
                    PdfPageRegion imagePdfRegion = htmlImageInfo.PdfRegions[0];
                    PdfPage imagePdfPage = document.Pages[imagePdfRegion.PageIndex];

                    // create the image element
                    PdfImage pdfImage = new PdfImage(imagePdfRegion.Rectangle, Server.MapPath("~") + @"\DemoFiles\Images\HiQPdfLogo_Modified.png");
                    pdfImage.ClipRectangle = imagePdfRegion.Rectangle;

                    imagePdfPage.Layout(pdfImage);
                }

                // write the PDF document to a memory buffer
                byte[] pdfBuffer = document.WriteToMemory();

                FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "ReplaceHtmlElements.pdf";

                return fileResult;

            }
            finally
            {
                if (document != null)
                    document.Close();
            }
        }
    }
}