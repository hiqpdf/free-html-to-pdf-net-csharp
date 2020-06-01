using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class FillAndSaveFormsController : Controller
    {
        // GET: FillAndSaveForms
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePdf(FormCollection collection)
        {
            string pdfFileWithForm = Server.MapPath("~") + @"\DemoFiles\Pdf\PdfWithForm.pdf";

            // load the PDF document with form from file
            PdfDocument document = PdfDocument.FromFile(pdfFileWithForm);

            // set a demo serial number
            document.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            // get the Check Box field by name from form fields collection and set its value
            PdfFormField checkBoxField = document.Form.Fields["cb"];
            if (checkBoxField != null)
                checkBoxField.Value = collection["checkBoxChecked"] != null;

            // get the Text Box field by name from form fields collection and set its value
            PdfFormField textBoxField = document.Form.Fields["tb"];
            if (textBoxField != null)
                textBoxField.Value = collection["textBoxText"];

            // get the List Box field by name from form fields collection and set its value
            PdfFormField listBoxField = document.Form.Fields["lb"];
            if (listBoxField != null)
                listBoxField.Value = collection["dropDownListListBoxValue"];

            // get the Combo Box field by name from form fields collection and set its value
            PdfFormField comboBoxField = document.Form.Fields["combo"];
            if (comboBoxField != null)
                comboBoxField.Value = collection["dropDownListComboBoxValue"];

            // get the Radio Buttons Group field by name from form fields collection and set its value
            PdfFormField radioGroupField = document.Form.Fields["rg"];
            if (radioGroupField != null)
                radioGroupField.Value = collection["SelectedRadionButton"] == "radioButton1" ? "rb1" : "rb2";

            try
            {
                // write the PDF document to a memory buffer
                byte[] pdfBuffer = document.WriteToMemory();

                FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "FillForms.pdf";

                return fileResult;
            }
            finally
            {
                document.Close();
            }
        }
    }
}