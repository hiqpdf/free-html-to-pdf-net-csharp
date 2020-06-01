using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class PdfHeadersAndFootersController : Controller
    {
        FormCollection m_formCollection;

        // GET: PdfHeadersAndFooters
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConvertToPdf(FormCollection collection)
        {
            m_formCollection = collection;

            // create the HTML to PDF converter
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // set a demo serial number
            htmlToPdfConverter.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            // set the default header and footer of the document
            SetHeader(htmlToPdfConverter.Document);
            SetFooter(htmlToPdfConverter.Document);

            // set a handler for PageCreatingEvent where to configure the PDF document pages
            htmlToPdfConverter.PageCreatingEvent += new PdfPageCreatingDelegate(htmlToPdfConverter_PageCreatingEvent);

            try
            {
                byte[] pdfBuffer = htmlToPdfConverter.ConvertUrlToMemory(collection["textBoxUrl"]);

                FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "PdfHeadersAndFooters.pdf";

                return fileResult;
            }
            finally
            {
                // dettach from PageCreatingEvent event
                htmlToPdfConverter.PageCreatingEvent -= new PdfPageCreatingDelegate(htmlToPdfConverter_PageCreatingEvent);
            }
        }

        void htmlToPdfConverter_PageCreatingEvent(PdfPageCreatingParams eventParams)
        {
            PdfPage pdfPage = eventParams.PdfPage;
            int pdfPageNumber = eventParams.PdfPageNumber;

            if (pdfPageNumber == 1)
            {
                // set the header and footer visibility in first page
                pdfPage.DisplayHeader = m_formCollection["checkBoxDisplayHeaderInFirstPage"] != null;
                pdfPage.DisplayFooter = m_formCollection["checkBoxDisplayFooterInFirstPage"] != null;
            }
            else if (pdfPageNumber == 2)
            {
                // set the header and footer visibility in second page
                pdfPage.DisplayHeader = m_formCollection["checkBoxDisplayHeaderInSecondPage"] != null;
                pdfPage.DisplayFooter = m_formCollection["checkBoxDisplayFooterInSecondPage"] != null;

                if (pdfPage.DisplayHeader && m_formCollection["checkBoxCustomizedHeaderInSecondPage"] != null)
                {
                    // override the default document header in this page
                    // with a customized header of 200 points in height
                    pdfPage.CreateHeaderCanvas(200);

                    // layout a HTML document in header
                    PdfHtml htmlInHeader = new PdfHtml("http://www.hiqpdf.com");
                    htmlInHeader.FitDestHeight = true;
                    pdfPage.Header.Layout(htmlInHeader);

                    // create a border for the customized header
                    PdfRectangle borderRectangle = new PdfRectangle(0, 0, pdfPage.Header.Width - 1, pdfPage.Header.Height - 1);
                    borderRectangle.LineStyle.LineWidth = 0.5f;
                    borderRectangle.ForeColor = System.Drawing.Color.Navy;
                    pdfPage.Header.Layout(borderRectangle);
                }
            }
        }

        private void SetHeader(PdfDocumentControl htmlToPdfDocument)
        {
            // enable header display
            htmlToPdfDocument.Header.Enabled = true;

            // set header height
            htmlToPdfDocument.Header.Height = 50;

            float pdfPageWidth = htmlToPdfDocument.PageOrientation == PdfPageOrientation.Portrait ?
                                        htmlToPdfDocument.PageSize.Width : htmlToPdfDocument.PageSize.Height;

            float headerWidth = pdfPageWidth - htmlToPdfDocument.Margins.Left - htmlToPdfDocument.Margins.Right;
            float headerHeight = htmlToPdfDocument.Header.Height;

            // set header background color
            htmlToPdfDocument.Header.BackgroundColor = System.Drawing.Color.WhiteSmoke;

            string headerImageFile = Server.MapPath("~") + @"\DemoFiles\Images\HiQPdfLogo.png";
            PdfImage logoHeaderImage = new PdfImage(5, 5, 40, System.Drawing.Image.FromFile(headerImageFile));
            htmlToPdfDocument.Header.Layout(logoHeaderImage);

            // layout HTML in header
            PdfHtml headerHtml = new PdfHtml(50, 5, @"<span style=""color:Navy; font-family:Times New Roman; font-style:italic"">
                            Quickly Create High Quality PDFs with </span><a href=""http://www.hiqpdf.com"">HiQPdf</a>", null);
            headerHtml.FitDestHeight = true;
            htmlToPdfDocument.Header.Layout(headerHtml);

            // create a border for header

            PdfRectangle borderRectangle = new PdfRectangle(1, 1, headerWidth - 2, headerHeight - 2);
            borderRectangle.LineStyle.LineWidth = 0.5f;
            borderRectangle.ForeColor = System.Drawing.Color.Navy;
            htmlToPdfDocument.Header.Layout(borderRectangle);
        }

        private void SetFooter(PdfDocumentControl htmlToPdfDocument)
        {
            // enable footer display
            htmlToPdfDocument.Footer.Enabled = true;

            // set footer height
            htmlToPdfDocument.Footer.Height = 50;

            // set footer background color
            htmlToPdfDocument.Footer.BackgroundColor = System.Drawing.Color.WhiteSmoke;

            float pdfPageWidth = htmlToPdfDocument.PageOrientation == PdfPageOrientation.Portrait ?
                                        htmlToPdfDocument.PageSize.Width : htmlToPdfDocument.PageSize.Height;

            float footerWidth = pdfPageWidth - htmlToPdfDocument.Margins.Left - htmlToPdfDocument.Margins.Right;
            float footerHeight = htmlToPdfDocument.Footer.Height;

            // layout HTML in footer
            PdfHtml footerHtml = new PdfHtml(5, 5, @"<span style=""color:Navy; font-family:Times New Roman; font-style:italic"">
                            Quickly Create High Quality PDFs with </span><a href=""http://www.hiqpdf.com"">HiQPdf</a>", null);
            footerHtml.FitDestHeight = true;
            htmlToPdfDocument.Footer.Layout(footerHtml);

            if (m_formCollection["checkBoxDisplayPageNumbersInFooter"] != null)
            {
                if (m_formCollection["checkBoxPageNumbersInHtml"] == null)
                {
                    // add page numbering in a text element
                    System.Drawing.Font pageNumberingFont = new System.Drawing.Font(new System.Drawing.FontFamily("Times New Roman"),
                                    8, System.Drawing.GraphicsUnit.Point);
                    PdfText pageNumberingText = new PdfText(5, footerHeight - 12, "Page {CrtPage} of {PageCount}", pageNumberingFont);
                    pageNumberingText.HorizontalAlign = PdfTextHAlign.Center;
                    pageNumberingText.EmbedSystemFont = true;
                    pageNumberingText.ForeColor = System.Drawing.Color.DarkGreen;
                    htmlToPdfDocument.Footer.Layout(pageNumberingText);
                }
                else
                {
                    // add page numbers in HTML - more flexible but less efficient than text version
                    PdfHtmlWithPlaceHolders htmlWithPageNumbers = new PdfHtmlWithPlaceHolders(5, footerHeight - 20,
                        "Page <span style=\"font-size: 16px; color: blue; font-style: italic; font-weight: bold\">{CrtPage}</span> of <span style=\"font-size: 16px; color: green; font-weight: bold\">{PageCount}</span>", null);
                    htmlToPdfDocument.Footer.Layout(htmlWithPageNumbers);
                }
            }

            string footerImageFile = Server.MapPath("~") + @"\DemoFiles\Images\HiQPdfLogo.png";
            PdfImage logoFooterImage = new PdfImage(footerWidth - 40 - 5, 5, 40, System.Drawing.Image.FromFile(footerImageFile));
            htmlToPdfDocument.Footer.Layout(logoFooterImage);

            // create a border for footer
            PdfRectangle borderRectangle = new PdfRectangle(1, 1, footerWidth - 2, footerHeight - 2);
            borderRectangle.LineStyle.LineWidth = 0.5f;
            borderRectangle.ForeColor = System.Drawing.Color.DarkGreen;
            htmlToPdfDocument.Footer.Layout(borderRectangle);
        }
    }
}