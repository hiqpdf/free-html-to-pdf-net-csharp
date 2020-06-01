using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class PdfDocumentSecuritySettingsController : Controller
    {
        FormCollection m_formCollection;

        // GET: PdfDocumentSecuritySettings
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePdf(FormCollection collection)
        {
            m_formCollection = collection; 

            // create an empty PDF document
            PdfDocument document = new PdfDocument();

            // set a demo serial number
            document.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            // set encryption mode
            document.Security.EncryptionMode = GetSelectedEncryptionMode();
            // set encryption level
            document.Security.EncryptionLevel = GetSelectedEncryptionLevel();

            // set open password
            document.Security.OpenPassword = collection["textBoxOpenPassword"];
            // set permissions password
            document.Security.PermissionsPassword = collection["textBoxPermissionsPassword"];

            // set PDF document permissions
            document.Security.AllowPrinting = collection["checkBoxAllowPrint"] != null;
            document.Security.AllowCopyContent = collection["checkBoxAllowCopy"] != null;
            document.Security.AllowEditContent = collection["checkBoxAllowEdit"] != null;
            document.Security.AllowEditAnnotations = collection["checkBoxAllowEditAnnotations"] != null;
            document.Security.AllowFormFilling = collection["checkBoxAllowFillForms"] != null;

            // set a default permissions password if an open password was set without settings a permissions password
            // or if any of the permissions does not have the default value
            if (document.Security.PermissionsPassword == String.Empty &&
               (document.Security.OpenPassword != String.Empty || !IsDefaultPermission(document.Security)))
            {
                document.Security.PermissionsPassword = "admin";
            }
            
            // add a page to document with the default size and orientation
            PdfPage page1 = document.AddPage(PdfPageSize.A4, new PdfDocumentMargins(0), PdfPageOrientation.Portrait);

            // an object to be set with HTML layout info after conversion
            PdfLayoutInfo htmlLayoutInfo = null;
            try
            {
                // create the HTML object from URL
                PdfHtml htmlObject = new PdfHtml(collection["textBoxUrl"]);

                // optionally wait an additional time before starting the conversion
                htmlObject.WaitBeforeConvert = 2;

                // layout the HTML object in PDF
                htmlLayoutInfo = page1.Layout(htmlObject);

                // write the PDF document to a memory buffer
                byte[] pdfBuffer = document.WriteToMemory();

                FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "PdfDocumentSecuritySettings.pdf";

                return fileResult;
            }
            finally
            {
                document.Close();
            }
        }

        private bool IsDefaultPermission(PdfDocumentSecurity pdfSecurity)
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