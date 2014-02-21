using System;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace PassPhotoMVC.Helpers
{
    /// <summary>
    /// Functions to manipulate uploaded images (resizing / cropping)
    /// </summary>
    /// 

    public class PhotoUtils
    {

        /// <summary>
        /// Handle uploading of images, calls resizing for creating standard size images
        /// based on AppSettings parameters
        /// </summary>
        /// <param name="postedfile"></param>
        /// <param name="targetfilename"></param>
        /// <param name="folder"></param>
        /// <param name="maxsize"></param>
        /// <param name="hmindimension"></param>
        /// <returns></returns>
        public String UploadImage(HttpPostedFileBase postedfile, String targetfilename, String folder, int maxsize = 262144, int hmindimension = 0)
        {
            String result = "";
            // if hmindimension=0 use default settings
            if (hmindimension == 0)
            {
                hmindimension = Convert.ToInt32(Settings.AppSettings.uploadheight);
            }                     
            Boolean fileOK = false;
            int fileSize = postedfile.ContentLength;            
            String maxk = ((int)((double)maxsize / 1024)).ToString();
            if  (fileSize > 0 & targetfilename.Length > 0)
            {
                String fileExtension = System.IO.Path.GetExtension(postedfile.FileName).ToLower();
                String[] allowedExtensions = { ".jpg", ".jpeg" };
                for (int i = 0; i <= allowedExtensions.Length - 1; i++)
                {
                    if (allowedExtensions[i].Equals(fileExtension))
                    {
                        fileOK = true;
                    }
                }
                if (fileOK)
                {
                    if (fileSize < maxsize)
                    {
                        try
                        {
                            result = ResizeImageUpload(postedfile.InputStream, targetfilename, folder, hmindimension);
                        }
                        catch(Exception ex)
                        {
                            result = "ERROR: File could not be uploaded <br>" + ex.Message;
                        }
                    }
                    else
                    {
                        result = "ERROR: File is larger than " + maxk + "K. Please upload a smaller image";
                    }
                }
                else
                {
                    result = "ERROR: Cannot accept files of this type.";
                }
            }
            else
            {
                result = "ERROR: Cannot upload photos without valid targetfilename.";
            }
            return result;
        }

        /// <summary>
        /// Handles resizing of uploaded images so all images are scaled to a standard height
        /// </summary>
        /// <param name="inputfilestream"></param>
        /// <param name="finalfilename"></param>
        /// <param name="folderpath"></param>
        /// <param name="hmindimension"></param>
        /// <returns></returns>
        public String ResizeImageUpload(Stream inputfilestream, String finalfilename, String folderpath, int hmindimension)
        {
            String result = "";
            // enforce final uploaded images to have a fixed height            
            int newStillWidth, newStillHeight;
            int ori1;
            Image originalimg;
            try
            {
                originalimg = System.Drawing.Image.FromStream(inputfilestream);
                if (originalimg.Width > originalimg.Height)
                {
                    // landscape rules
                    ori1 = originalimg.Height;
                    newStillHeight = hmindimension;
                    newStillWidth = (int)((double)originalimg.Width * hmindimension / ori1);
                }
                else
                {
                    // portrait rules
                     ori1 = originalimg.Width;
                    newStillHeight = hmindimension;
                    newStillWidth = (int)((double)newStillHeight * originalimg.Width / originalimg.Height);
                }
                Bitmap still = new Bitmap(newStillWidth, newStillHeight);
                Graphics gr_dest_still = Graphics.FromImage(still);
                SolidBrush sb = new SolidBrush(System.Drawing.Color.White);
                gr_dest_still.FillRectangle(sb, 0, 0, still.Width, still.Height);
                gr_dest_still.DrawImage(originalimg, 0, 0, still.Width, still.Height);
                try
                {
                    ImageCodecInfo codecencoder = GetEncoder("image/jpeg");
                    int quality = 90;
                    EncoderParameters encodeparams = new EncoderParameters(1);
                    EncoderParameter qualityparam = new EncoderParameter(Encoder.Quality, quality);
                    encodeparams.Param[0] = qualityparam;
                    still.SetResolution(96, 96);
                    if (!folderpath.EndsWith("\\")){
                        folderpath += "\\";
                    }
                    still.Save(folderpath  + finalfilename, codecencoder, encodeparams);
                    result = "OK: File uploaded!";
                }
                catch(Exception ex)
                {
                    result = "ERROR: there was a problem saving the image. " + ex.Message;
                }
                if (still!=null)
                {
                    still.Dispose();                    
                }    
            }
            catch(Exception ex)
            {
                result = "ERROR: that was not an image we could process. " + ex.Message;
            }
            return result;
        }

        /// <summary>
        /// Handles cropping of images, scales the cropped images to standard dimensions
        /// configured in the AppSettings file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="sourcefolder"></param>
        /// <param name="targetfolder"></param>
        /// <param name="imgfolder"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="W"></param>
        /// <param name="H"></param>
        /// <returns></returns>
        public String CropImage(String filename, String sourcefolder, String targetfolder, String imgfolder, int X, int Y, int W, int H)
        {
            String result = "";           
            // enforce final cropped images to have fixed dimensions
            int croppedfinalw, croppedfinalh;
            croppedfinalh = Convert.ToInt32(Settings.AppSettings.passheight);
            croppedfinalw = Convert.ToInt32(Settings.AppSettings.passwidth);
            try
            {
                if (!imgfolder.EndsWith("\\"))
                {
                    imgfolder += "\\";
                }
                String sourcepath = imgfolder + sourcefolder + "\\";
                Bitmap image1 = (Bitmap)Image.FromFile(sourcepath + filename, true);
                Rectangle rect = new Rectangle(X, Y, W, H);
                Bitmap cropped = image1.Clone(rect, image1.PixelFormat);
                // dispose original image in case we need to overwrite it below
                if (image1 != null)
                {
                    image1.Dispose();                    
                }    
                Bitmap finalcropped= new Bitmap(croppedfinalw, croppedfinalh);
                Graphics gr_finalcropped  = Graphics.FromImage(finalcropped);
                SolidBrush sb = new SolidBrush(System.Drawing.Color.White);
                gr_finalcropped.FillRectangle(sb, 0, 0, finalcropped.Width, finalcropped.Height);
                gr_finalcropped.DrawImage(cropped, 0, 0, finalcropped.Width, finalcropped.Height);
                try
                {
                    ImageCodecInfo codecencoder  = GetEncoder("image/jpeg");
                    int quality = 92;
                    EncoderParameters encodeparams  = new EncoderParameters(1);
                    EncoderParameter qualityparam = new EncoderParameter(Encoder.Quality, quality);
                    encodeparams.Param[0] = qualityparam;
                    finalcropped.SetResolution(240, 240);
                    sourcepath = sourcepath.Replace(sourcefolder, targetfolder);
                    finalcropped.Save(sourcepath + filename, codecencoder, encodeparams);
                    result = "OK - File cropped";
                }
                catch(Exception ex)
                {
                    result = "ERROR: there was a problem saving the image. " + ex.Message;
                }
                if (cropped != null)
                {
                    cropped.Dispose();                    
                }
                if (finalcropped != null)
                {
                    finalcropped.Dispose();                    
                }
            }
            catch(Exception ex)
            {
                result = "ERROR: that was not an image we could process. " + ex.Message;
            }
            return result;
        }

        public ImageCodecInfo GetEncoder(String mimetype)
        {
            ImageCodecInfo result = null;
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
            {
                if (codec.MimeType == mimetype){
                    result = codec;
                }
            }
            return result;
        }

        public int CalculateResizedWidth(String filename, String folderpath, int newh)
        {
            int result = 0;
            if (!folderpath.EndsWith("\\"))
            {
                folderpath += "\\";
            }
            String fullpath = folderpath + filename;
            Bitmap image1 = (Bitmap)Image.FromFile(fullpath, true);
            if (image1 != null)
            {
                result = (int)((double)newh * image1.Width / image1.Height);
                image1.Dispose();                
            }    
            return result;
        }

        /// <summary>
        /// Updates the javascript code sent to the view to handle the cropping
        /// based on the dimensions of the source image used for cropping.
        /// </summary>
        /// <param name="neww"></param>
        /// <param name="newh"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public String UpdatePreviewJs(int neww, int newh, String filepath="")
        {
            String result = "";
            // read the default js source file 
            // this structure allows unit testing 
            // by passing a test filepath in the unit test
            if (filepath == "" || filepath==null)
            {
                HttpContext ctx = HttpContext.Current;
                if (ctx != null)
                {
                    filepath = ctx.Server.MapPath("~/js/tiffjcroppreset.js");
                }                
            }            
            result = System.IO.File.ReadAllText(filepath);
            int startselect = result.IndexOf("width: Math.round");
            int startvalue = result.IndexOf("(", startselect);
            int endvalue = result.IndexOf(")", startvalue);
            String selectvalue = result.Substring(startvalue, endvalue - startvalue + 1);
            String newvalue = "(rx*" + neww.ToString() + ")";
            result = result.Replace(selectvalue, newvalue);
            // change height based on source image
            startselect = result.IndexOf("height: Math.round");
            startvalue = result.IndexOf("(", startselect);
            endvalue = result.IndexOf(")", startvalue);
            selectvalue = result.Substring(startvalue, endvalue - startvalue + 1);
            newvalue = "(ry*" + newh.ToString() + ")";
            result = result.Replace(selectvalue, newvalue);
            // configure the prefixed aspect ratio
            // determine fixed image ratio from settings
            String ratio = Settings.AppSettings.passwidth + "/" + Settings.AppSettings.passheight;
            startselect = result.IndexOf("aspectRatio:");
            startvalue = result.IndexOf(" ", startselect);
            endvalue = result.IndexOf(",", startvalue);
            selectvalue = result.Substring(startvalue, endvalue - startvalue + 1);
            newvalue = " " + ratio + ",";
            result = result.Replace(selectvalue, newvalue);
            // configure default values for rx            
            startselect = result.IndexOf("var rx");
            startvalue = result.IndexOf("=", startselect);
            endvalue = result.IndexOf(";", startvalue);
            selectvalue = result.Substring(startvalue, endvalue - startvalue + 1);
            newvalue = "= " + Settings.AppSettings.previewwidth + " / c.w;";
            result = result.Replace(selectvalue, newvalue);            
            // configure default values for ry 
            startselect = result.IndexOf("var ry ");
            startvalue = result.IndexOf("=", startselect);
            endvalue = result.IndexOf(";", startvalue);
            selectvalue = result.Substring(startvalue, endvalue - startvalue + 1);
            newvalue = "= " + Settings.AppSettings.previewheight + " / c.h;";
            result = result.Replace(selectvalue, newvalue);
            // Configure Explicit Resizing 
            startselect = result.IndexOf("trueSize:");
            startvalue = result.IndexOf("[", startselect);
            endvalue = result.IndexOf("]", startvalue);
            selectvalue = result.Substring(startvalue, endvalue - startvalue + 1);
            newvalue = "[" + neww.ToString() + ", " + newh.ToString() + "]";
            result = result.Replace(selectvalue, newvalue);  
            return result;
        }      

    }   
    
}


