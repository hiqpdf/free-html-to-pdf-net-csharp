using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class HtmlFittingAndScalingOptionsController : Controller
    {
        FormCollection m_formCollection;

        // GET: HtmlFittingAndScalingOptions
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ConvertToPdf(FormCollection collection)
        {
            m_formCollection = collection;

            // create the HTML to PDF converter
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // set a demo serial number
            htmlToPdfConverter.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            // set browser width
            htmlToPdfConverter.BrowserWidth = int.Parse(collection["textBoxBrowserWidth"]);

            // set browser height if specified, otherwise use the default
            if (collection["textBoxBrowserHeight"].Length > 0)
                htmlToPdfConverter.BrowserHeight = int.Parse(collection["textBoxBrowserHeight"]);

            // set HTML Load timeout
            htmlToPdfConverter.HtmlLoadedTimeout = int.Parse(collection["textBoxLoadHtmlTimeout"]);

            // optionally wait an additional time before starting the conversion
            // it is not necessary to set this property if the HTML page is not updated after initial load
            htmlToPdfConverter.WaitBeforeConvert = 2;

            // set PDF page size and orientation
            htmlToPdfConverter.Document.PageSize = GetSelectedPageSize();
            htmlToPdfConverter.Document.PageOrientation = GetSelectedPageOrientation();

            // set PDF page size and orientation
            htmlToPdfConverter.Document.PageSize = GetSelectedPageSize();
            htmlToPdfConverter.Document.PageOrientation = GetSelectedPageOrientation();

            // set PDF page margins
            htmlToPdfConverter.Document.Margins = new PdfMargins(
                                        int.Parse(collection["textBoxLeftMargin"]), int.Parse(collection["textBoxRightMargin"]),
                                        int.Parse(collection["textBoxTopMargin"]), int.Parse(collection["textBoxBottomMargin"]));

            // set HTML location and size in PDF page
            if (collection["textBoxHtmlLeftLocation"].Length > 0)
                htmlToPdfConverter.Document.DestX = float.Parse(collection["textBoxHtmlLeftLocation"]);
            if (collection["textBoxHtmlTopLocation"].Length > 0)
                htmlToPdfConverter.Document.DestY = float.Parse(collection["textBoxHtmlTopLocation"]);
            if (collection["textBoxHtmlWidth"].Length > 0)
                htmlToPdfConverter.Document.DestWidth = float.Parse(collection["textBoxHtmlWidth"]);
            if (collection["textBoxHtmlHeight"].Length > 0)
                htmlToPdfConverter.Document.DestHeight = float.Parse(collection["textBoxHtmlHeight"]);

            // control if the HTML can be scaled to fit the PDF page width
            htmlToPdfConverter.Document.FitPageWidth = collection["checkBoxFitPageWidth"] != null;

            // control if the HTML can be enlarged to fit the PDF page width when FitPageWidth is true
            htmlToPdfConverter.Document.ForceFitPageWidth = collection["checkBoxForceFitPageWidth"] != null;

            // control if the PDF page can be resized to display the whole HTML content when FitPageWidth is false
            htmlToPdfConverter.Document.ResizePageWidth = collection["checkBoxResizePdfPage"] != null;

            // control if the HTML content can be scaled to fit the PDF page height
            htmlToPdfConverter.Document.FitPageHeight = collection["checkBoxFitPageHeight"] != null;

            // control if the whole HTML content will be rendered in one PDF page without scaling
            htmlToPdfConverter.Document.PostCardMode = collection["checkBoxPostCardMode"] != null;

            // convert HTML to PDF
            byte[] pdfBuffer = null;

            if (collection["UrlOrHtmlCode"] == "radioButtonConvertUrl")
            {
                // convert URL to a PDF memory buffer
                string url = collection["textBoxUrl"];

                pdfBuffer = htmlToPdfConverter.ConvertUrlToMemory(url);
            }
            else
            {
                // convert HTML code
                string htmlCode = collection["textBoxHtmlCode"];
                string baseUrl = collection["textBoxBaseUrl"];

                // convert HTML code to a PDF memory buffer
                pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(htmlCode, baseUrl);
            }

            FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "HtmlFittingAndScaling.pdf";

            return fileResult;
        }

        private PdfPageSize GetSelectedPageSize()
        {
            switch (m_formCollection["dropDownListPageSizes"])
            {
                case "A0":
                    return PdfPageSize.A0;
                case "A1":
                    return PdfPageSize.A1;
                case "A10":
                    return PdfPageSize.A10;
                case "A2":
                    return PdfPageSize.A2;
                case "A3":
                    return PdfPageSize.A3;
                case "A4":
                    return PdfPageSize.A4;
                case "A5":
                    return PdfPageSize.A5;
                case "A6":
                    return PdfPageSize.A6;
                case "A7":
                    return PdfPageSize.A7;
                case "A8":
                    return PdfPageSize.A8;
                case "A9":
                    return PdfPageSize.A9;
                case "ArchA":
                    return PdfPageSize.ArchA;
                case "ArchB":
                    return PdfPageSize.ArchB;
                case "ArchC":
                    return PdfPageSize.ArchC;
                case "ArchD":
                    return PdfPageSize.ArchD;
                case "ArchE":
                    return PdfPageSize.ArchE;
                case "B0":
                    return PdfPageSize.B0;
                case "B1":
                    return PdfPageSize.B1;
                case "B2":
                    return PdfPageSize.B2;
                case "B3":
                    return PdfPageSize.B3;
                case "B4":
                    return PdfPageSize.B4;
                case "B5":
                    return PdfPageSize.B5;
                case "Flsa":
                    return PdfPageSize.Flsa;
                case "HalfLetter":
                    return PdfPageSize.HalfLetter;
                case "Ledger":
                    return PdfPageSize.Ledger;
                case "Legal":
                    return PdfPageSize.Legal;
                case "Letter":
                    return PdfPageSize.Letter;
                case "Letter11x17":
                    return PdfPageSize.Letter11x17;
                case "Note":
                    return PdfPageSize.Note;
                default:
                    return PdfPageSize.A4;
            }
        }

        private PdfPageOrientation GetSelectedPageOrientation()
        {
            return (m_formCollection["dropDownListPageOrientations"] == "Portrait") ?
                PdfPageOrientation.Portrait : PdfPageOrientation.Landscape;
        }
    }
}