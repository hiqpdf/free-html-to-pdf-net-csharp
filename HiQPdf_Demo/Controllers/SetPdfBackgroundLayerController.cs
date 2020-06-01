using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class SetPdfBackgroundLayerController : Controller
    {
        FormCollection m_formCollection;

        // GET: SetPdfBackgroundLayer
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

            // attach to PageLayoutingEvent event raised right before layouting the HTML content in a PDF page
            htmlToPdfConverter.PageLayoutingEvent += new PdfPageLayoutingDelegate(htmlToPdfConverter_PageLayoutingEvent);

            // set PDF page margins
            htmlToPdfConverter.Document.Margins = new PdfMargins(
                                        int.Parse(collection["textBoxLeftMargin"]), int.Parse(collection["textBoxRightMargin"]),
                                        int.Parse(collection["textBoxTopMargin"]), int.Parse(collection["textBoxBottomMargin"]));

            try
            {
                byte[] pdfBuffer = htmlToPdfConverter.ConvertUrlToMemory(collection["textBoxUrl"]);

                FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "SetPdfBackground.pdf";

                return fileResult;
            }
            finally
            {
                // dettach from PageLayoutingEvent event
                htmlToPdfConverter.PageLayoutingEvent -= new PdfPageLayoutingDelegate(htmlToPdfConverter_PageLayoutingEvent);
            }
        }

        /// <summary>
        /// The PageLayoutingEvent event handler called before each PDF page is rendered by the converter
        /// </summary>
        /// <param name="eventParams">The event handler parameter giving information about the PDF page being rendered 
        /// and the rectangle in page that will be rendered</param>
        void htmlToPdfConverter_PageLayoutingEvent(PdfPageLayoutingParams eventParams)
        {
            // The PDF page being rendered
            PdfPage crtPage = eventParams.PdfPage;

            // draw a colored rectangle in the background of the PDF page
            PdfRectangle backColorRect = new PdfRectangle(0, 0, crtPage.DrawableRectangle.Width, crtPage.DrawableRectangle.Height);
            backColorRect.BackColor = System.Drawing.Color.FromArgb(255, int.Parse(m_formCollection["textBoxR"]), int.Parse(m_formCollection["textBoxG"]), int.Parse(m_formCollection["textBoxB"]));
            crtPage.Layout(backColorRect);

            // draw a 2 points orange line under the rendered content in page
            System.Drawing.PointF leftBottom = new System.Drawing.PointF(eventParams.LayoutingBounds.Left, eventParams.LayoutingBounds.Bottom + 1);
            System.Drawing.PointF rightBottom = new System.Drawing.PointF(eventParams.LayoutingBounds.Right, eventParams.LayoutingBounds.Bottom + 1);
            PdfLine bottomLine = new PdfLine(leftBottom, rightBottom);
            bottomLine.LineStyle.LineWidth = 2.0f;
            bottomLine.ForeColor = System.Drawing.Color.OrangeRed;
            crtPage.Layout(bottomLine);
        }
    }
}