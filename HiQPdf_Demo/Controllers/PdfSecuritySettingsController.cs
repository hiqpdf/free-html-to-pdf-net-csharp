using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class PdfSecuritySettingsController : Controller
    {
        FormCollection m_formCollection;

        // GET: PdfSecuritySettings
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

            // set encryption mode
            htmlToPdfConverter.Document.Security.EncryptionMode = GetSelectedEncryptionMode();
            // set encryption level
            htmlToPdfConverter.Document.Security.EncryptionLevel = GetSelectedEncryptionLevel();

            // set open password
            htmlToPdfConverter.Document.Security.OpenPassword = collection["textBoxOpenPassword"];
            // set permissions password
            htmlToPdfConverter.Document.Security.PermissionsPassword = collection["textBoxPermissionsPassword"];

            // set PDF document permissions
            htmlToPdfConverter.Document.Security.AllowPrinting = collection["checkBoxAllowPrint"] != null;
            htmlToPdfConverter.Document.Security.AllowCopyContent = collection["checkBoxAllowCopy"] != null;
            htmlToPdfConverter.Document.Security.AllowEditContent = collection["checkBoxAllowEdit"] != null;
            htmlToPdfConverter.Document.Security.AllowEditAnnotations = collection["checkBoxAllowEditAnnotations"] != null;
            htmlToPdfConverter.Document.Security.AllowFormFilling = collection["checkBoxAllowFillForms"] != null;

            // set a default permissions password if an open password was set without settings a permissions password
            // or if any of the permissions does not have the default value
            if (htmlToPdfConverter.Document.Security.PermissionsPassword == String.Empty &&
               (htmlToPdfConverter.Document.Security.OpenPassword != String.Empty || !IsDefaultPermission(htmlToPdfConverter.Document.Security)))
            {
                htmlToPdfConverter.Document.Security.PermissionsPassword = "admin";
            }

            // convert URL to a PDF memory buffer
            string url = collection["textBoxUrl"];

            byte[] pdfBuffer = htmlToPdfConverter.ConvertUrlToMemory(url);

            FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "PdfSecuritySettings.pdf";

            return fileResult;
        }

        private bool IsDefaultPermission(PdfSecurity pdfSecurity)
        {
            if (!pdfSecurity.AllowAssembling)
                return false;
            if (!pdfSecurity.AllowCopyAccessibilityContent)
                return false;
            if (!pdfSecurity.AllowCopyContent)
                return false;
            if (!pdfSecurity.AllowEditAnnotations)
                return false;
            if (!pdfSecurity.AllowEditContent)
                return false;
            if (!pdfSecurity.AllowFormFilling)
                return false;
            if (!pdfSecurity.AllowHighResolutionPrinting)
                return false;
            if (!pdfSecurity.AllowPrinting)
                return false;

            return true;
        }

        private PdfEncryptionMode GetSelectedEncryptionMode()
        {
            switch (m_formCollection["dropDownListEncryptionMode"])
            {
                case "RC4":
                    return PdfEncryptionMode.RC4;
                case "AES":
                    return PdfEncryptionMode.AES;
                default:
                    return PdfEncryptionMode.RC4;
            }
        }

        private PdfEncryptionLevel GetSelectedEncryptionLevel()
        {
            switch (m_formCollection["dropDownListEncryptionLevel"])
            {
                case "Low":
                    return PdfEncryptionLevel.Low;
                case "High":
                    return PdfEncryptionLevel.High;
                case "Very High":
                    return PdfEncryptionLevel.VeryHigh;
                default:
                    return PdfEncryptionLevel.High;
            }
        }
    }
}