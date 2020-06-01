using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class RasterizePdfPagesToImagesController : Controller
    {
        FormCollection m_formCollection;

        // GET: RasterizePdfPagesToImages
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RasterizeToImages(FormCollection collection)
        {
            m_formCollection = collection;

            // get the PDF file
            string pdfFile = Server.MapPath("~") + @"\DemoFiles\Pdf\InputPdf.pdf";

            // create the PDF rasterizer
            PdfRasterizer pdfRasterizer = new PdfRasterizer();

            // set a demo serial number
            pdfRasterizer.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            // set the output images color space
            pdfRasterizer.ColorSpace = GetColorSpace();

            // set the rasterization resolution in DPI
            pdfRasterizer.DPI = int.Parse(collection["textBoxDPI"]);

            int fromPdfPageNumber = int.Parse(collection["textBoxFromPage"]);
            int toPdfPageNumber = collection["textBoxToPage"].Length > 0 ? int.Parse(collection["textBoxToPage"]) : 0;

            byte[] imageBuffer = null;

            if (collection["checkBoxToTiff"] != null)
            {
                // convert the PDF document to a multipage TIFF image in a memory buffer
                // the TIFF images can also be produced in a file using the RasterizeToTiffFile method
                imageBuffer = pdfRasterizer.RasterizeToTiff(pdfFile, fromPdfPageNumber, toPdfPageNumber);
            }
            else
            {
                // rasterize a range of pages of the PDF document to memory in .NET Image objects
                // the images can also be produced to a folder using the RasterizeToImageFiles method
                // or they can be produced one by one using the RaisePageRasterizedEvent method
                PdfPageRasterImage[] pageImages = pdfRasterizer.RasterizeToImageObjects(pdfFile, fromPdfPageNumber, toPdfPageNumber);

                // return if no page was rasterized
                if (pageImages.Length == 0)
                    return Redirect("/RasterizePdfPagesToImages");

                // get the first page image bytes in a buffer            
                try
                {
                    // get the .NET Image object
                    System.Drawing.Image imageObject = pageImages[0].ImageObject;

                    // get the image data in a buffer
                    imageBuffer = GetImageBuffer(imageObject);
                }
                finally
                {
                    // dispose the page images
                    for (int i = 0; i < pageImages.Length; i++)
                        pageImages[i].Dispose();
                }
            }

            if (collection["checkBoxToTiff"] != null)
            {
                FileResult fileResult = new FileContentResult(imageBuffer, "image/tiff");
                fileResult.FileDownloadName = "PageImage.tiff";

                return fileResult;
            }
            else
            {
                FileResult fileResult = new FileContentResult(imageBuffer, "image/png");
                fileResult.FileDownloadName = "PageImage.png";

                return fileResult;
            }
        }

        private RasterImageColorSpace GetColorSpace()
        {
            switch (m_formCollection["dropDownListColorSpace"])
            {
                case "RGB":
                    return RasterImageColorSpace.Rgb;
                case "Gray Scale":
                    return RasterImageColorSpace.GrayScale;
                case "Black and White":
                    return RasterImageColorSpace.BlackWhite;
                default:
                    return RasterImageColorSpace.Rgb;
            }
        }

        private byte[] GetImageBuffer(System.Drawing.Image imageObject)
        {
            // create a memory stream where to save the image
            System.IO.MemoryStream imageMemoryStream = new System.IO.MemoryStream();

            // save the image to memory stream
            imageObject.Save(imageMemoryStream, System.Drawing.Imaging.ImageFormat.Png);

            // get a copy of the image buffer to allow image disposing
            byte[] imageBuffer = new byte[imageMemoryStream.Length];
            Array.Copy(imageMemoryStream.GetBuffer(), imageBuffer, imageBuffer.Length);

            // close the memory stream
            imageMemoryStream.Close();

            return imageBuffer;
        }
    }
}