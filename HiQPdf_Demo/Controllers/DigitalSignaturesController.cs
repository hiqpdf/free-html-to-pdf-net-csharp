using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Drawing;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class DigitalSignaturesController : Controller
    {
        // GET: DigitalSignatures
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePdf(FormCollection collection)
        {
            // create a PDF document
            PdfDocument document = new PdfDocument();

            // set a demo serial number
            document.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            // create a page in document
            PdfPage page1 = document.AddPage();

            // create the true type fonts that can be used in document text
            Font sysFont = new Font("Times New Roman", 10, System.Drawing.GraphicsUnit.Point);
            PdfFont pdfFont = document.CreateFont(sysFont);
            PdfFont pdfFontEmbed = document.CreateFont(sysFont, true);

            float crtYPos = 20;
            float crtXPos = 5;

            // add a title to PDF document
            PdfText titleTextTransImage = new PdfText(crtXPos, crtYPos,
                    "Click the image below to open the digital signature", pdfFontEmbed);
            titleTextTransImage.ForeColor = Color.Navy;
            PdfLayoutInfo textLayoutInfo = page1.Layout(titleTextTransImage);

            crtYPos += textLayoutInfo.LastPageRectangle.Height + 10;

            // layout a PNG image with alpha transparency
            PdfImage transparentPdfImage = new PdfImage(crtXPos, crtYPos, Server.MapPath("~") + @"\DemoFiles\Images\HiQPdfLogo_small.png");
            PdfLayoutInfo imageLayoutInfo = page1.Layout(transparentPdfImage);

            // apply a digital sgnature over the image
            PdfCertificatesCollection pdfCertificates = PdfCertificatesCollection.FromFile(Server.MapPath("~") + @"\DemoFiles\Pfx\hiqpdf.pfx", "hiqpdf");
            PdfDigitalSignature digitalSignature = new PdfDigitalSignature(pdfCertificates[0]);
            digitalSignature.SigningReason = "My signing reason";
            digitalSignature.SigningLocation = "My signing location";
            digitalSignature.SignerContactInfo = "My contact info";
            document.AddDigitalSignature(digitalSignature, imageLayoutInfo.LastPdfPage, imageLayoutInfo.LastPageRectangle);

            try
            {
                // write the PDF document to a memory buffer
                byte[] pdfBuffer = document.WriteToMemory();

                FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "DigitalSignatures.pdf";

                return fileResult;
            }
            finally
            {
                document.Close();
            }
        }
    }
}