using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class ConvertHtmlToImageController : Controller
    {
        FormCollection m_formCollection;

        // GET: ConvertHtmlToImage
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ConvertToImage(FormCollection collection)
        {
            m_formCollection = collection;

            // create the HTML to Image converter
            HtmlToImage htmlToImageConverter = new HtmlToImage();

            // set a demo serial number
            htmlToImageConverter.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            // set browser width
            htmlToImageConverter.BrowserWidth = int.Parse(collection["textBoxBrowserWidth"]);

            // set browser height if specified, otherwise use the default
            if (collection["textBoxBrowserHeight"].Length > 0)
                htmlToImageConverter.BrowserHeight = int.Parse(collection["textBoxBrowserHeight"]);

            // set HTML Load timeout
            htmlToImageConverter.HtmlLoadedTimeout = int.Parse(collection["textBoxLoadHtmlTimeout"]);

            // set whether the resulted image is transparent (has effect only when the output format is PNG)
            htmlToImageConverter.TransparentImage = (collection["dropDownListImageFormat"] == "PNG") ?
                        collection["checkBoxTransparentImage"] != null : false;

            // set triggering mode; for WaitTime mode set the wait time before convert
            switch (collection["dropDownListTriggeringMode"])
            {
                case "Auto":
                    htmlToImageConverter.TriggerMode = ConversionTriggerMode.Auto;
                    break;
                case "WaitTime":
                    htmlToImageConverter.TriggerMode = ConversionTriggerMode.WaitTime;
                    htmlToImageConverter.WaitBeforeConvert = int.Parse(collection["textBoxWaitTime"]);
                    break;
                case "Manual":
                    htmlToImageConverter.TriggerMode = ConversionTriggerMode.Manual;
                    break;
                default:
                    htmlToImageConverter.TriggerMode = ConversionTriggerMode.Auto;
                    break;
            }

            // convert to image
            System.Drawing.Image imageObject = null;
            string imageFormatName = collection["dropDownListImageFormat"].ToLower();
            string imageFileName = String.Format("HtmlToImage.{0}", imageFormatName);

            if (collection["UrlOrHtmlCode"] == "radioButtonConvertUrl")
            {
                // convert URL
                string url = collection["textBoxUrl"];

                imageObject = htmlToImageConverter.ConvertUrlToImage(url)[0];
            }
            else
            {
                // convert HTML code
                string htmlCode = collection["textBoxHtmlCode"];
                string baseUrl = collection["textBoxBaseUrl"];

                imageObject = htmlToImageConverter.ConvertHtmlToImage(htmlCode, baseUrl)[0];
            }

            // get the image buffer in the selected image format
            byte[] imageBuffer = GetImageBuffer(imageObject);

            // the image object rturned by converter can be disposed
            imageObject.Dispose();

            // inform the browser about the binary data format
            string mimeType = imageFormatName == "jpg" ? "jpeg" : imageFormatName;

            FileResult fileResult = new FileContentResult(imageBuffer, "image/" + mimeType);
            fileResult.FileDownloadName = imageFileName;

            return fileResult;
        }

        private byte[] GetImageBuffer(System.Drawing.Image imageObject)
        {
            // create a memory stream where to save the image
            System.IO.MemoryStream imageMemoryStream = new System.IO.MemoryStream();

            // save the image to memory stream
            imageObject.Save(imageMemoryStream, GetSelectedImageFormat());

            // get a copy of the image buffer to allow image disposing
            byte[] imageBuffer = new byte[imageMemoryStream.Length];
            Array.Copy(imageMemoryStream.GetBuffer(), imageBuffer, imageBuffer.Length);

            // close the memory stream
            imageMemoryStream.Close();

            return imageBuffer;
        }

        private System.Drawing.Imaging.ImageFormat GetSelectedImageFormat()
        {
            switch (m_formCollection["dropDownListImageFormat"])
            {
                case "PNG":
                    return System.Drawing.Imaging.ImageFormat.Png;
                case "JPG":
                    return System.Drawing.Imaging.ImageFormat.Jpeg;
                case "BMP":
                    return System.Drawing.Imaging.ImageFormat.Bmp;
                default:
                    return System.Drawing.Imaging.ImageFormat.Png;
            }
        }
    }
}