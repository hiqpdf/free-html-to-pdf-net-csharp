using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Text;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class ExtractTextFromPdfController : Controller
    {
        FormCollection m_formCollection;

        // GET: ExtractTextFromPdf
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ExtractText(FormCollection collection)
        {
            m_formCollection = collection;

            // get the PDF file
            string pdfFile = Server.MapPath("~") + @"\DemoFiles\Pdf\InputPdf.pdf";

            // create the PDF text extractor
            PdfTextExtract pdfTextExtract = new PdfTextExtract();

            // set a demo serial number
            pdfTextExtract.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            // set the text extraction mode
            pdfTextExtract.TextExtractMode = GetTextExtractMode();

            int fromPdfPageNumber = int.Parse(collection["textBoxFromPage"]);
            int toPdfPageNumber = collection["textBoxToPage"].Length > 0 ? int.Parse(collection["textBoxToPage"]) : 0;

            // extract the text from a range of pages of the PDF document
            string text = pdfTextExtract.ExtractText(pdfFile, fromPdfPageNumber, toPdfPageNumber);

            // get UTF-8 bytes
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(text);

            // the UTF-8 marker
            byte[] utf8Marker = new byte[] { 0xEF, 0xBB, 0xBF };

            // the text document bytes with UTF-8 marker followed by UTF-8 bytes
            byte[] bytes = new byte[utf8Bytes.Length + utf8Marker.Length];
            Array.Copy(utf8Marker, 0, bytes, 0, utf8Marker.Length);
            Array.Copy(utf8Bytes, 0, bytes, utf8Marker.Length, utf8Bytes.Length);

            FileResult fileResult = new FileContentResult(bytes, "text/plain; charset=UTF-8");
            fileResult.FileDownloadName = "ExtractedText.txt";

            return fileResult;
        }

        private PdfTextExtractMode GetTextExtractMode()
        {
            switch (m_formCollection["dropDownListExtractMode"])
            {
                case "Keep Positioning":
                    return PdfTextExtractMode.KeepPositioning;
                case "Keep Reading Order":
                    return PdfTextExtractMode.KeepReadingOrder;
                default:
                    return PdfTextExtractMode.KeepPositioning;
            }
        }
    }
}