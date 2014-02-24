using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PassPhotoMVC.Helpers;
using System.Collections;

namespace PassPhotoMVC.Controllers
{
    public class PassPhotoController : Controller
    {
        // initialize helper library and local variables
        PhotoUtils myUtils = new PhotoUtils();
        // max file size allowed to upload
        int maxSize = Convert.ToInt32(Settings.AppSettings.maxuploadsize);
        // all images are normalized to this height when uploaded
        // they are scaled up or down        
        int hminRaw = Convert.ToInt32(Settings.AppSettings.uploadheight);
        // required dimensions (w,h) of final cropped images
        int hminCropped = Convert.ToInt32(Settings.AppSettings.passheight);
        int wminCropped = Convert.ToInt32(Settings.AppSettings.passwidth);
        int imgUploadPreview = Convert.ToInt32(Settings.AppSettings.uploadpreviewh);
        int prevw = Convert.ToInt32(Settings.AppSettings.previewwidth);
        int prevh = Convert.ToInt32(Settings.AppSettings.previewheight);
        
        /// <summary>
        /// Determines parameters for the default view
        /// </summary>
        /// <param name="jsfilepath">only needed to override the preset value</param>
        /// <returns></returns>
        public ActionResult Index(String jsFilePath="")
        {            
            
            String RootPath = Request.Path;
            ViewBag.RootPath = RootPath;
            ViewBag.Height = prevh.ToString();
            ViewBag.Width = prevw.ToString();
            ViewBag.PreviewJSMarkup = myUtils.UpdatePreviewJs(prevw, prevh, jsFilePath);            
            
            return View();
        }

        /// <summary>
        /// Redirects to start screen of UI if end user tries to call this action using GET
        /// instead of POST
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadImage()
        {
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Handles uploading of files and resizing uploaded files to standard dimensions
        /// </summary>
        /// <param name="mypostedfile"></param>
        /// <param name="targetfilename"></param>
        /// <param name="targetfolder"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase myPostedFile, String targetFilename, String targetFolder = "raw", String srcimgfolder = "", String jsFilePath = "")
        {
            // This value is sent back to the view to accomodate relative URLs when in post action
            String rootPath = Request.Path.Replace("PassPhoto/UploadImage", "");
            ViewBag.RootPath = rootPath;
            if (myPostedFile != null && myPostedFile.ContentLength > 0) 
            {
                // default value for img folder is taken from current server context unless specified otherwise in input params
                if (srcimgfolder == "")
                {
                    srcimgfolder = Server.MapPath("~/uploaded_images/" + targetFolder + "/");
                }
                else
                {
                    if (!srcimgfolder.EndsWith("\\"))
                    {
                        srcimgfolder += "\\";
                    }
                    srcimgfolder += targetFolder;
                }
                // preset name of file while testing
                // refactor after tested
                targetFilename = "image_upload_test.jpg";
                ViewBag.Message = myUtils.UploadImage(myPostedFile, targetFilename, srcimgfolder, maxSize, hminRaw);
                if (ViewBag.Message == "OK: File uploaded!")
                {   
                    // return values required to modify UI   
                    ViewBag.ImageName = targetFilename;
                    ViewBag.ImageUrl = "uploaded_images/" + targetFolder + "/" + targetFilename;
                    ViewBag.Height = imgUploadPreview.ToString();
                    ViewBag.Width = myUtils.CalculateResizedWidth(targetFilename, srcimgfolder, imgUploadPreview).ToString();
                    ViewBag.PreviewDisplay = "normal";
                    int jsWidth = myUtils.CalculateResizedWidth(targetFilename, srcimgfolder, hminRaw);
                    ViewBag.PreviewJSMarkup = myUtils.UpdatePreviewJs(jsWidth, hminRaw, jsFilePath);
                }                
            }
            else
            {
                ViewBag.Message = "Error - Invalid File - Uploaded file is null.";
            }
            return View("Index");              
        }

        /// <summary>
        /// Redirects to start screen of UI if end user tries to call this action using GET
        /// instead of POST
        /// </summary>
        /// <returns></returns>
        public ActionResult CropImage()
        {
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Handles cropping of files keeping the aspect radio and standard dimensions
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="folder"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="W"></param>
        /// <param name="H"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CropImage(String filename, String folder, String X, String Y, String W, String H, String srcimgfolder = "", String jsFilePath = "")
        {
            String rootPath = Request.Path.Replace("PassPhoto/CropImage", "");
            ViewBag.RootPath = rootPath;
            String missingpar = "";            
            // validate required post parameters and return proper error message is something is missing
            if (filename == null )
            {
                missingpar = "filename";
            }
            if (folder == null)
            {
                if (missingpar != "")
                {
                    missingpar += ", ";
                }
                missingpar = "folder";
            }            
            if (missingpar.Length >0)
            {
                String ermsg1 = "Error - missing parameter";
                if (missingpar.Contains(","))
                {
                    ermsg1 += "s";
                }
                ViewBag.Message = ermsg1 + ": " + missingpar;
            }
            else
            {
                // convert posted values to int
                int W1 = Convert.ToInt32(W);
                int H1 = Convert.ToInt32(H);
                int X1 = Convert.ToInt32(X);
                int Y1 = Convert.ToInt32(Y);
                // default value for srcimagefolder will be determined at runtime from server context
                if (srcimgfolder == "")
                {
                    srcimgfolder = Server.MapPath("~/uploaded_images/");
                }
                else
                {
                    if (!srcimgfolder.EndsWith("\\"))
                    {
                        srcimgfolder += "\\";
                    }                   
                }
                String targetfolder = "cropped";
                ViewBag.Message = myUtils.CropImage(filename, folder, targetfolder, srcimgfolder, X1, Y1, W1, H1);
                if (ViewBag.Message == "OK - File cropped")
                {    
                    ViewBag.Height = imgUploadPreview.ToString();
                    ViewBag.Width = myUtils.CalculateResizedWidth(filename, srcimgfolder + targetfolder, imgUploadPreview).ToString();
                    ViewBag.ImageName = filename;
                    ViewBag.PreviewDisplay = "normal";                    
                    ViewBag.ImageUrl = "uploaded_images/cropped/" + filename;
                    ViewBag.PreviewJSMarkup = myUtils.UpdatePreviewJs(wminCropped, hminCropped, jsFilePath);
                }
            }           
            return View("Index");            
        }
        
    }
}