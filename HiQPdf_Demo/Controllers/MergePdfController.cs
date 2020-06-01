using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class MergePdfController : Controller
    {
        // GET: MergePdf
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePdf(FormCollection collection)
        {
            // create an empty document which will become the final document after merge
            PdfDocument resultDocument = new PdfDocument();

            // set a demo serial number
            resultDocument.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            // load the first document to be merged from a file
            string pdfFile1 = Server.MapPath("~") + @"\DemoFiles\Pdf\WikiHtml.pdf";
            PdfDocument document1 = PdfDocument.FromFile(pdfFile1);

            // load the second document to be merged from a FileStream to exemplify the PDF loading from a stream
            // The stream must remain open until the result document is saved. The stream is closed when the document2 
            // will be closed
            string pdfFile2 = Server.MapPath("~") + @"\DemoFiles\Pdf\WikiPdf.pdf";
            System.IO.FileStream pdfStream = new System.IO.FileStream(pdfFile2, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
            PdfDocument document2 = PdfDocument.FromStream(pdfStream);

            // add the two documents to the result document
            resultDocument.AddDocument(document1);
            resultDocument.AddDocument(document2);

            try
            {
                // write the PDF document to a memory buffer
                byte[] pdfBuffer = resultDocument.WriteToMemory();

                FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "MergePdf.pdf";

                return fileResult;
            }
            finally
            {
                // close the result document
                resultDocument.Close();
                // close the merged documents
                // this will also close the pdfStream from which document 2 was loaded 
                document1.Close();
                document2.Close();
            }
        }
    }
}