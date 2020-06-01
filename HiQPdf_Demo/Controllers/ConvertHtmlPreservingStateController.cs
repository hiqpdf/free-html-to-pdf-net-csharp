using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.IO;
using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class ConvertHtmlPreservingStateController : Controller
    {
        // GET: ConvertHtmlPreservingState
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConvertThisViewToPdf(FormCollection formCollection)
        {
            // add the custom value to view data
            ViewDataDictionary viewData = new ViewDataDictionary();
            viewData.Add("MyTextValue", formCollection["textBoxText"]);
            viewData.Add("MyDropDownListValue", formCollection["dropDownListValues"]);

            viewData.Add("displaySessionVariable", "true");
            Session["SessionVariable"] = formCollection["textBoxCrtSessionVariable"];

            // get the HTML code of this view
            string htmlToConvert = RenderViewAsString("Index", viewData);

            // the base URL to resolve relative images and css
            String thisViewUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
            String baseUrl = thisViewUrl.Substring(0, thisViewUrl.Length - "ConvertHtmlPreservingState/ConvertThisViewToPdf".Length);

            // instantiate the HiQPdf HTML to PDF converter
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // render the HTML code as PDF in memory
            byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(htmlToConvert, baseUrl);

            FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "ThisViewToPdf.pdf";

            return fileResult;
        }

        public string RenderViewAsString(string viewName, ViewDataDictionary viewData)
        {
            // create a string writer to receive the HTML code
            StringWriter stringWriter = new StringWriter();

            // get the view to render
            ViewEngineResult viewResult = ViewEngines.Engines.FindView(ControllerContext, viewName, null);
            // create a context to render a view based on a model
            ViewContext viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    viewData,
                    new TempDataDictionary(),
                    stringWriter
                    );

            // render the view to a HTML code
            viewResult.View.Render(viewContext, stringWriter);

            // return the HTML code
            return stringWriter.ToString();
        }

        [HttpPost]
        public ActionResult ConvertAnotherViewToPdf(FormCollection formCollection)
        {
            // set a session variable to be used in the the converted view
            Session["MySessionVariable"] = formCollection["textBoxAnotherSessionVariable"];

            // get the About view HTML code
            string htmlToConvert = RenderViewAsString("AnotherView", new ViewDataDictionary());

            // the base URL to resolve relative images and css
            String thisViewUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
            String baseUrl = thisViewUrl.Substring(0, thisViewUrl.Length - "ConvertHtmlPreservingState/ConvertAnotherViewToPdf".Length);

            // instantiate the HiQPdf HTML to PDF converter
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // render the HTML code as PDF in memory
            byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(htmlToConvert, baseUrl);

            // send the PDF document to browser
            FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "AnotherViewToPdf.pdf";

            return fileResult;
        }
    }
}