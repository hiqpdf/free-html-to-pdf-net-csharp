using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Drawing;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class SearchTextInPdfController : Controller
    {
        // GET: SearchTextInPdf
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchText(FormCollection collection)
        {
            // get the PDF file
            string pdfFile = Server.MapPath("~") + @"\DemoFiles\Pdf\InputPdf.pdf";

            // get the text to search
            string textToSearch = collection["textBoxTextToSearch"];

            // create the PDF text extractor
            PdfTextExtract pdfTextExtract = new PdfTextExtract();

            // set a demo serial number
            pdfTextExtract.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            int fromPdfPageNumber = int.Parse(collection["textBoxFromPage"]);
            int toPdfPageNumber = collection["textBoxToPage"].Length > 0 ? int.Parse(collection["textBoxToPage"]) : 0;

            // search the text in PDF document
            PdfTextSearchItem[] searchTextInstances = pdfTextExtract.SearchText(pdfFile, textToSearch,
                        fromPdfPageNumber, toPdfPageNumber, collection["checkBoxMatchCase"] != null, collection["checkBoxMatchWholeWord"] != null);

            // load the PDF file to highlight the searched text
            PdfDocument pdfDocument = PdfDocument.FromFile(pdfFile);

            // set a demo serial number
            pdfDocument.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            // highlight the searched text in PDF document
            foreach (PdfTextSearchItem searchTextInstance in searchTextInstances)
            {
                PdfRectangle pdfRectangle = new PdfRectangle(searchTextInstance.BoundingRectangle);

                // set rectangle color and opacity
                pdfRectangle.BackColor = Color.Yellow;
                pdfRectangle.Opacity = 30;

                // highlight the text
                pdfDocument.Pages[searchTextInstance.PdfPageNumber - 1].Layout(pdfRectangle);
            }

            // write the modified PDF document
            try
            {
                // write the PDF document to a memory buffer
                byte[] pdfBuffer = pdfDocument.WriteToMemory();

                FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "SearchText.pdf";

                return fileResult;
            }
            finally
            {
                pdfDocument.Close();
            }
        }
    }
}